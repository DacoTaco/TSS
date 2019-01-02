//select row - change colour, open item
function SelectTaskRow(taskID,evt) {
    return loadTaskPage(taskID);
}

function SelectUserRow(UserID,evt) {
    return loadUserPage(UserID);
}

//function to handle pressing enter on the textboxes
function onSearchKeyEnter(evt, callback) {
    var keycode;

    //retrieve keycode. evt used for decent browsers, window.event for IE/Edge.
    //keycode is for IS, which is for the rest.
    evt = evt || window.event;
    var keycode = evt.keyCode || evt.which;

    if (keycode == 13) {
        //enter was pressed
        //use all known ways to prevent the event from executing normally and reget the grid
        evt.preventDefault();
        evt.returnValue = false;
        evt.cancel = true;

        //run the function that was given
        if (callback)
            callback();
    }
    return false;
}

//setting the current active tab
function SetActiveTab(name) {
    var control = $("#hidTABControl");
    control.val(name);
    PushNewPropertyValue("Index.aspx", "Tab", name, false);
    return true;
}

var closeModelEventListener = function () { };
var SaveFn = function () { };
var CloseFn = CloseModal;
//loading the task page and displaying the modal
function loadTaskPage(taskID, evt) {
    if (taskID != null || taskID.length > 0)
        var parameters = { "TaskID": taskID };

    LoadPageDivIntoDiv("EditTask.aspx",
        "#EditTask",
        "#editModal",
        parameters,
        function() { $("#EditWindow").modal('toggle'); });

    var exists = (typeof IsTaskReadOnly === "function");
    var ret = true;
    if (exists) {
        var ret = IsTaskReadOnly();
    }

    var SaveBtn = document.getElementById("SaveModal");
    if (ret == null || ret == true)
    {
        SaveBtn.disabled = true;
        SaveFn = function () { };
        CloseFn = CloseModal;
    }
    else
    {
        SaveBtn.disabled = false;
        SaveFn = SaveTaskFn;
        CloseFn = CloseTaskModal;
    }
    closeModelEventListener = CloseFn;
    $("#SaveModal").attr("onclick", "SaveFn(); return false;");
    $("#CloseBtn").attr("onclick", "CloseFn(); return false;");
    $("#EditWindow").on("hide.bs.modal", closeModelEventListener);

    return false;
}

//load user page and display window!
function loadUserPage(UserID, evt) {
    if (UserID != null || UserID.length > 0)
        var parameters = { "UserID": UserID };

    LoadPageDivIntoDiv("EditUser.aspx",
        "#EditUser",
        "#editModal",
        parameters,
        function() { $("#EditWindow").modal(); });

    var exists = (typeof IsUserReadOnly === "function");
    var ret = true;
    if (exists) {
        var ret = IsUserReadOnly();
    }

    var SaveBtn = document.getElementById("SaveModal");
    if (ret == null || ret == true) {
        SaveBtn.disabled = true;
        SaveFn = function () { };
        CloseFn = CloseModal;
    } else {
        SaveBtn.disabled = false;
        SaveFn = SaveUserFn;
        CloseFn = CloseUserModal;       
    }

    closeModelEventListener = CloseFn;
    $("#SaveModal").attr("onclick", "SaveFn(); return false;");
    $("#CloseBtn").attr("onclick", "CloseFn(); return false;");
    $("#EditWindow").on("hide.bs.modal", closeModelEventListener);

    return false;

}

//saving and closing
function SaveTaskFn() {
    var ret = SaveTask();
    if (ret == true) {
        SetTaskClosed();
        CloseModal();
    }
    return false;
}

function CloseTaskModal() {
    SetTaskClosed();
    CloseModal();
}

function SaveUserFn() {
    var ret = SaveUser();
    if (ret == true) {
        SetUserClosed();
        CloseModal();
    }
    return false;
}

function CloseUserModal() {
    SetUserClosed();
    return CloseModal();
}

function CloseModal() {
    var dropdown = document.getElementById("DropDownSorting");
    var department = dropdown.value;
    var textbox = document.getElementById("searchbar");
    if (textbox)
        textbox.value = "";

    $("#SaveModal").attr("onclick", "return false;");
    $("#CloseBtn").attr("onclick", "return false;");
    $("#EditWindow").off("hide.bs.modal", closeModelEventListener);

    //it looks like somewhere down the line i had issues with asp controls not firing after opening the model.
    //this doesn't seem to be a problem anymore since most of the stuff we do is javascript communicating and reloading(ajax) soooo... just reload active div? :')
    //window.location.href = window.location.href;

    var control = $("#hidTABControl");
    var DivToReload = control[0].value;

    LoadPageDivIntoDiv("Index.aspx", DivToReload, DivToReload, null, null);
}