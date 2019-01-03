//retrieve task list
function searchTaskPage() {
    return getTasksPage("-2");
}

function getTasksPage(department, callback) {
    var contains = $("#searchbar").val();
    var depNumber = Number(department);
    var parameters = { "Search": "", "depID": -2 };

    if (contains.length > 0)
        parameters.Search = contains;
    if (department != null && depNumber != null && depNumber > -5)
        parameters.depID = depNumber;

    var dropdown = document.getElementById("DropDownSorting");
    if (depNumber > -1)
        dropdown.value = depNumber;
    else
        dropdown.value = 0;

    LoadPageDivIntoDiv("Index.aspx", "#TaskGrid", "#TaskGrid", parameters, callback);

}

//when changing department from dropdown
function onDepartmentChanged() {
    var dropdown = document.getElementById("DropDownSorting");
    var department = dropdown.value;
    var textbox = document.getElementById("searchbar");
    if (textbox)
        textbox.value = "";
    getTasksPage(department);
    return false;
}

//calls upon the ASP.NET function that will push the changes to the task which is keeping track of all changes
function PushTaskPropertyChange(PropertyName, value, Async) {
    return PushNewPropertyValue("EditTask.aspx", PropertyName, value, Async);
}

//call upon the ASP.NET function to retrieve a value from the Task
function GetTaskProperty(PropertyName) {
    var ret;
    var ajaxFn = function() {
        $.ajax({
            type: "POST",
            url: "EditTask.aspx/GetTaskProperty",
            data: "{PropertyName:'" + PropertyName + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            cache: false,
            success: function(data) {
                ret = data;
                return true;
            },
            error: function(jqXHR, textStatus, errorThrown) {
                alert("Error retrieving Property '" + PropertyName + "'\n\r error : " + jqXHR.responseText);
                return false;
            }
        });
    };

    ajaxFn();
    return ret.d;
}

//call upon the ASP.NET function to save the Task!
function SaveTask() {
    var ret;
    var ajaxFn = function() {
        $.ajax({
            type: "POST",
            url: "EditTask.aspx/SaveTask",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            cache: false,
            success: function(data) {
                if ((data) && (data.d)) {
                    alert(data.d);
                    ret = false;
                    return false;
                } else {
                    ret = true;
                    return true;
                }
            },
            error: function(jqXHR, textStatus, errorThrown) {
                alert("Error saving Task!\n\rerror : " + jqXHR.responseText);
                ret = false;
                return false;
            }
        });
    };
    ajaxFn();
    return ret;
}

//update task description
function updateTaskDescription() {
    var Descr = $("#txtDescription").val();
    var PropertyName = "Description";

    return PushTaskPropertyChange(PropertyName, Descr);
}

//update Task Reporter
function updateTaskReporter() {
    var value = $("#txtReporter").val();
    var PropertyName = "Reporter";

    return PushTaskPropertyChange(PropertyName, value);
}

// etc etc...
function updateTaskUrguency() {
    var PropertyName = "IsUrguent";
    var value = document.getElementById("chkUrguent").checked;

    return PushTaskPropertyChange(PropertyName, value);
}

function updateTaskTechnician() {
    var PropertyName = "Technician";
    var value = document.getElementById("selectTechnicians").value;
    var ret = PushTaskPropertyChange(PropertyName, value, false);

    var StatusID = GetTaskProperty("StatusID");

    document.getElementById("selectTaskState").value = StatusID;

    return ret;
}

function updateTaskState() {
    var PropertyName = "StatusID";
    var value = document.getElementById("selectTaskState").value;

    var ret = PushTaskPropertyChange(PropertyName, value, false);

    var TechnicianID = GetTaskProperty("TechnicianID");
    document.getElementById("selectTechnicians").value = TechnicianID;

    return ret;
}

function updateTaskMachine() {
    var PropertyName = "Machine";
    var value = document.getElementById("selectMachines").value;

    var ret = PushTaskPropertyChange(PropertyName, value, false);
    return ret;
}

function updateTaskLocation() {
    var PropertyName = "Location";
    var value = document.getElementById("selectLocations").value;

    var ret = PushTaskPropertyChange(PropertyName, value, true);
    return ret;
}

function updateTaskDepartment() {

    //first update the department
    var PropertyName = "DepartmentID";
    var value = document.getElementById("selectDepartments").value;

    var ret = PushTaskPropertyChange(PropertyName, value, false);

    var LocationObject = GetTaskProperty("Location");

    //then replace the location dropdown :P
    //we can't reload the page because it would cause shit to reset losing all data.
    //SO, we just get the locations using a webmethod and parse it in javascript.
    var ret;
    var ajaxGetLocations = function() {
        $.ajax({
            type: "POST",
            url: "EditTask.aspx/GetLocations",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            cache: false,
            success: function(data) {
                ret = data;
                return true;
            },
            error: function(jqXHR, textStatus, errorThrown) {
                alert("Error saving changes of Description, error : " + jqXHR.responseText);
                return false;
            }
        });
    };
    ajaxGetLocations();

    //now that we have the list of locations we repopulate the select
    var control = document.getElementById("selectLocations");
    var i;
    for (i = control.options.length - 1; i >= 0; i--) {
        control.remove(i);
    }
    i = 0;
    if (ret != null) {
        //get array from the returned data...
        var array = ret.d;
        var length = array.length;

        //looping time!
        //this is fun as the structure of the objects is the same as in C#. so we get the ID and the Name, perfect!
        for (i = 0; i < length; i++) {
            var option = document.createElement("option");
            var location = array[i];

            option.value = location.ID;
            if (location.ID == LocationObject.ID)
                option.selected = true;
            option.text = location.Description;

            control.add(option);
        }
    }

}

function AddTaskNote() {
    var PropertyName = "Notes";
    var note = $("#NoteBox").val();

    if (!note)
        return false;

    var ret = PushTaskPropertyChange(PropertyName, note, false);

    $("#NoteBox").val("");

    var notes = GetTaskProperty("strNotes");

    $("#Notes").val(notes);

    return false;
}

function SetTaskClosed() {
    var ret = true;
    var CloseTask = function() {
        $.ajax({
            type: "POST",
            url: "EditTask.aspx/CloseTask",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            cache: false,
            success: function(data) {
                if ((data) && (data.d)) {
                    ret = true;
                    return true;
                } else {
                    ret = false;
                    return false;
                }
            },
            error: function(jqXHR, textStatus, errorThrown) {
                alert("Error Closing Task in Database!\n\rerror : " + jqXHR.responseText);
                ret = true;
                return true;
            }
        });
    };

    CloseTask();
    return ret;
}

function IsTaskReadOnly() {
    var ret;
    var ajaxFn = function() {
        $.ajax({
            type: "POST",
            url: "EditTask.aspx/TaskReadOnly",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            cache: false,
            success: function(data) {
                if ((data) && (data.d)) {
                    alert(data.d);
                    ret = true;
                    return true;
                } else {
                    ret = false;
                    return false;
                }
            },
            error: function(jqXHR, textStatus, errorThrown) {
                alert("Error requesting ReadOnly state of Task!\n\rerror : " + jqXHR.responseText);
                ret = true;
                return true;
            }
        });
    };
    ajaxFn();
    return ret;
}

function AddTaskPhoto(input) {
    var reader = null;
    var file = null;
    var timeOutId = null;
    var setLoading = function() {
        var loadingdiv = document.getElementById("loadingTaskDiv");
        if (loadingdiv)
            loadingdiv.hidden = false;
    };

    if (input.files && input.files[0]) {
        reader = new FileReader();
        file = input.files[0];

        reader.onload = function(e) {
            var PropertyName = "Photos";
            var fileURL = e.target.result;
            var photoName = String(fileURL);
            var ret = PushTaskPropertyChange(PropertyName, photoName, false);
            if (ret === true) {
                var ul = document.getElementById("imagesList");
                var li = document.createElement("li");
                var ancor = document.createElement("a");
                var image = document.createElement("img");

                li.setAttribute("style", "display:inline;max-height:inherit;");

                ancor.setAttribute("style",
                    "height:inherit;margin-bottom:0;max-height:inherit;min-height:inherit;padding-right:5px;");
                ancor.setAttribute("data-lightbox", "TaskImages");
                ancor.setAttribute("href", fileURL);

                image.setAttribute("src", fileURL);
                image.setAttribute("style", "height:inherit;margin-bottom:0; max-height:inherit;min-height:inherit;");
                image.setAttribute("alt", e.target.filename);

                ancor.appendChild(image);
                li.appendChild(ancor);
                ul.appendChild(li);
            } else {
                alert("error adding photo to task!");
            }

            var loadingdiv = document.getElementById("loadingTaskDiv");
            if (loadingdiv)
                loadingdiv.hidden = true;
            clearTimeout(timeOutId);

            return ret;
        };
        reader.filename = file.name;

        //timeOutId = setTimeout(setLoading, 100);
        setLoading();
        reader.readAsDataURL(input.files[0]);
    }
}