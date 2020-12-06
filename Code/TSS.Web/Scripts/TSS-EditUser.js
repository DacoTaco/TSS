//retrieve task list
function SearchUserPage() {
    return getUserPage("Unknown");
}

function getUserPage(role, callback) {
    var contains = $("#txtSearchUser").val();
    var timeOutId = 0;

    if (role == null && contains.length > 0)
        var parameters = { "SearchUser": contains };
    else
        var parameters = { "UserRole": role };

    var dropdown = document.getElementById("selectUserType");
    if (role != "Unknown")
        dropdown.value = role;
    else
        dropdown.value = 0;

    LoadPageDivIntoDiv("Index.aspx", "#UserGrid", "#UserGrid", parameters, callback);

}

//calls upon the ASP.NET function that will push the changes to the User which is keeping track of all changes
function PushUserPropertyChange(PropertyName, value, Async) {
    return PushNewPropertyValue("EditUser.aspx", PropertyName, value, Async);
}

function onUserTypeChanged() {
    var dropdown = document.getElementById("selectUserType");
    var role = dropdown.value;
    var textbox = document.getElementById("txtSearchUser");
    if (textbox)
        textbox.value = "";

    getUserPage(role);
}

function SaveUser() {
    var ret;
    var ajaxFn = function() {
        $.ajax({
            type: "POST",
            url: "EditUser.aspx/SaveUser",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            cache: false,
            success: function(data) {
                if ((data) && (data.d)) {
                    alert(data.d);
                    ret = false;
                    var loadingdiv = document.getElementById("loadingUserDiv");
                    if (loadingdiv)
                        loadingdiv.hidden = true;
                    return false;
                } else {
                    ret = true;
                    return true;
                }
            },
            error: function(jqXHR, textStatus, errorThrown) {
                alert("Error saving User!\n\rerror : " + jqXHR.responseText);
                ret = false;
                return false;
            }
        });
    };
    var setLoading = function() {
        var loadingdiv = document.getElementById("loadingUserDiv");
        if (loadingdiv)
            loadingdiv.hidden = false;
    };
    setLoading();
    ajaxFn();
    return ret;
}

function SetUserClosed() {
    var ret = true;
    var CloseUser = function() {
        $.ajax({
            type: "POST",
            url: "EditUser.aspx/CloseUser",
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
                alert("Error Closing User in Database!\n\rerror : " + jqXHR.responseText);
                ret = true;
                return true;
            }
        });
    };

    CloseUser();
    return ret;
}

function IsUserReadOnly() {
    var ret;
    var ajaxFn = function() {
        $.ajax({
            type: "POST",
            url: "EditUser.aspx/UserReadOnly",
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
                alert("Error requesting ReadOnly state of User!\n\rerror : " + jqXHR.responseText);
                ret = true;
                return true;
            }
        });
    };
    ajaxFn();
    return ret;
}

function updateDepartment() {
    var PropertyName = "Department";
    var value = document.getElementById("selectDepartment").value;

    var ret = PushUserPropertyChange(PropertyName, value, false);

    if (ret === false) {
        alert("Failed to set user's department!");
    }
    return ret;
}

function updateUsername() {
    var txtUsername = $("#txtUsername");
    if (txtUsername != null) {
        var value = txtUsername.val();
        var property = "UserName";
        var ret = PushUserPropertyChange(property, value, false);
        if (ret === true) {
            return true;
        } else {
            alert("failure changing Username!");
            return false;
        }
    }
    return ret;

}

function updatePassword() {
    var txtUsername = $("#txtPassword");
    if (txtUsername != null) {
        var value = txtUsername.val();
        var property = "Password";
        var ret = PushUserPropertyChange(property, value, false);
        if (ret === true) {
            return true;
        } else {
            alert("failure changing Password!");
            return false;
        }
    }
    return ret;
}

function updateActive() {
    var checkbox = $("#UserActive");
    if (checkbox) {
        var value = checkbox.is(":checked");
        var property = "IsActive";

        var ret = PushUserPropertyChange(property, value, false);
        if (ret === true) {
            return true;
        } else {
            alert("failure changing user enabled state!");
            return false;
        }
    }
}

function IsRoleAdmin(role)
{  
    if (role == null)
        return false;

    var ret = false;
    var ajaxFn = function() {
        $.ajax({
            type: "GET",
            url: "api/User/Role/" + role + "/IsAdmin",
            async: false,
            cache: "false",
            success: function(data) {
                if (data == true) {
                    ret = true;
                    return true;
                } else {
                    ret = false;
                    return false;
                }
            },
            error: function(jqXHR, textStatus, errorThrown) {
                alert("Error checking role, error : " + jqXHR.responseText);
            }
        });
    };

    ajaxFn();
    return ret;
}

function updateRole(control, role, index) {
    var ret = false;
    var isChecked = false;

    if (index < 0)
        return false;

    if (control != null) {
        var Property = "Roles";
        isChecked = control.checked;
        var value = {};
        value.Role = role;
        value.IsChecked = isChecked;
        var ret = PushUserPropertyChange(Property, JSON.stringify(value), false);
        if (ret === false) {
            alert("failure changing user role");
            return false;
        }
    }
    //basically, we get a list of all checkboxes, check if the activated checkbox is the admin one, and if it is enable/disable all the others
    var table = document.getElementById("RolesTable");
    var checkboxes = table.getElementsByTagName("input");

    if (checkboxes == null || checkboxes.length < 1 || checkboxes.length < index)
        return true;

    if (IsRoleAdmin(role)) {
        for (var i = 0; i < checkboxes.length; i++) {
            var input = checkboxes[i];
            if (input.type != "checkbox" || i == index)
                continue;

            input.disabled = isChecked;
            if (isChecked == true) {
                input.checked = true;
                //no need to push the change,the server did it by itself!
                //makes it so much faster
            }
        }
    }

    return ret;
}

function AddUserPhoto(input) {
    var reader = null;
    var file = null;
    var timeOutId = null;
    var setLoading = function() {
        var loadingdiv = document.getElementById("loadingUserDiv");
        if (loadingdiv)
            loadingdiv.hidden = false;
    };

    if (input && input.files && input.files[0]) {
        reader = new FileReader();
        file = input.files[0];

        reader.onload = function(e) {
            var ret = false;
            var PropertyName = "Photo";
            var fileURL = e.target.result;
            var photoName = String(fileURL);
            //todo, get orientation Nd flip image if needed

            var ret = PushUserPropertyChange(PropertyName, photoName, false);
            if (ret === true) {
                var image = document.getElementById("UserPhoto");

                image.setAttribute("src", fileURL);
                image.setAttribute("alt", e.target.filename);
            } else {
                alert("error adding photo to User!");
            }


            var loadingdiv = document.getElementById("loadingUserDiv");
            if (loadingdiv)
                loadingdiv.hidden = true;
            clearTimeout(timeOutId);

            return ret;
        };
        reader.filename = file.name;

        setLoading();
        reader.readAsDataURL(input.files[0]);
    }
}