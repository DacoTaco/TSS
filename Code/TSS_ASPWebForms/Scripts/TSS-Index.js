//select row - change colour, open item
function SelectTaskRow(taskID,rowID,evt)
{
    return SelectRow(rowID, "TaskView", function () { loadTaskPage(taskID); });
}
function SelectRow(rowID, tableName, callback) {
    /*rowID += 1;
    var table = document.getElementById(tableName);
    var tr = document.getElementById(tableName).rows;
    var size = tr.length;
    for (var i = 1;i < size; i++) {
        var row = table.rows[i];
        if (row.style.backgroundColor != 'transparent')
            row.style.backgroundColor = 'transparent';
        if(i == rowID)
        {
            table.rows[rowID].style.backgroundColor = "#A1DCF2";
        }
    }*/
    if (callback)
        callback();
}
function SelectUserRow(UserID,rowID,evt)
{
    return SelectRow(rowID, "UserGrid", function () { loadUserPage(UserID);})
}

//function to handle pressing enter on the textboxes
function onSearchKeyEnter(evt,callback) {
    var keycode;

    //retrieve keycode. evt used for decent browsers, window.event for IE/Edge.
    //keycode is for IS, which is for the rest.
    evt = evt || window.event;
    var keycode = evt.keyCode || evt.which;

    if (keycode == 13) {
        //use all known ways to prevent the event from executing normally and reget the grid
        evt.preventDefault();
        evt.returnValue = false;
        evt.cancel = true;

        //run the function that was given
        if (callback)
            callback();
        //getTasksPage("-2");
    }
    return false;
}

//setting the current active tab
function SetActiveTab(name)
{
    var control = $("#hidTABControl");
    control.val(name);
    return true;
}
//loading the task page and displaying the modal
function loadTaskPage(taskID,evt)
{
    if (taskID != null || taskID.length > 0)
        var parameters = { "TaskID": taskID };

    LoadPageDivIntoDiv("EditTask.aspx", "#EditTask", "#editModal", parameters, function () { $("#EditWindow").modal(); });

    var exists = (typeof IsTaskReadOnly === 'function');
    var ret = true;
    if (exists)
    {
        var ret = IsTaskReadOnly();
    }

    var SaveBtn = document.getElementById("SaveModal");
    if (ret == null || ret == true)
    {
        SaveBtn.disabled = true;
        SaveBtn.setAttribute("onclick", "return false;");
        $("#CloseBtn").attr("onclick", "CloseModal(); return false;");
        $('#EditWindow').on('hide.bs.modal', function () { CloseModal(); });
    }
    else
    {
        SaveBtn.disabled = false;
        $("#SaveModal").attr("onclick", "SaveTaskFn(); return false;");
        $("#CloseBtn").attr("onclick", "CloseTaskModal(); return false;");
        $('#EditWindow').on('hide.bs.modal', function () { CloseTaskModal(); });
    }

    return false;
}
//load user page and display window!
function loadUserPage(UserID,evt)
{
    if (UserID != null || UserID.length > 0)
        var parameters = { "UserID": UserID };

    LoadPageDivIntoDiv("EditUser.aspx", "#EditUser", "#editModal", parameters, function () { $("#EditWindow").modal(); });

    var exists = (typeof IsUserReadOnly === 'function');
    var ret = true;
    if (exists) {
        var ret = IsUserReadOnly();
    }

    var SaveBtn = document.getElementById("SaveModal");
    if (ret == null || ret == true) {
        SaveBtn.disabled = true;
        SaveBtn.setAttribute("onclick", "return false;");
        $("#CloseBtn").attr("onclick", "CloseModal(); return false;");
        $('#EditWindow').on('hide.bs.modal', function () { CloseModal(); });
    }
    else {
        SaveBtn.disabled = false;
        $("#SaveModal").attr("onclick", "SaveUserFn(); return false;");
        $("#CloseBtn").attr("onclick", "CloseUserModal(); return false;");
        $('#EditWindow').on('hide.bs.modal', function () { CloseUserModal(); });
    }

    return false;

}

//saving and closing
function SaveTaskFn()
{
    var ret = SaveTask();
    if (ret == true)
    {
        SetTaskClosed();
        CloseModal();
    }  
    return false;
}
function CloseTaskModal()
{
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
function CloseUserModal()
{
    SetUserClosed();
    return CloseModal();
}

function CloseModal()
{
    var dropdown = document.getElementById("DropDownSorting");
    var department = dropdown.value;
    var textbox = document.getElementById("searchbar");
    if (textbox)
        textbox.value = "";

    //reload required because otherwise asp controls start not firing their code behind shit?!?!
    var setLoading = function () {
        $("#loadingDiv").show();
    }

    //timeOutId = setTimeout(setLoading, 100);
    //location.reload(); gives the "do you wanna resend postback data?", the other one doesn't :D
    //location.reload();
    //var href = Math.random();
    //location.href = location.href + '?' + href;
    //window.location = window.location;
    window.location.href = window.location.href;
    //getTasksPage(department, function () { $("#EditWindow").modal.CloseModal(); });
    /*LoadPageDivIntoDiv("Index.aspx", "#MainContainer", "#MainContainer", null, function () { $("#EditWindow").modal.CloseModal(); });
    
    if(typeof setTab === 'function')
    {
        setTab();
    }*/

}