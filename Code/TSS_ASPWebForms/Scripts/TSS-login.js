//AJAX function to get the LoginUser page and load its login div into this page's login div,passing the index ID with it
function getLoginPage(id)
{
    LoadPageDivIntoDiv("LoginUser.aspx", "#Login", "#Login", { "ID": id }, function () { $('#form1').attr('onkeypress', "javascript:return WebForm_FireDefaultButton(event, 'btnLogin')"); });
}

//AJAX function to start the WebMethod Login on the LoginUser page. 
//we retrieve the userID from the dropdown, get the password from textbox and try to login !
function login()
{
    var pass = $("#txtPassword").val();
    var e = document.getElementById("userList");
    var userID = e.options[e.selectedIndex].value;
    var success = false;
    var ajaxFn = function (callback) {
        var ret = false;
        $.ajax({
            type: "POST",
            url: "LoginUser.aspx/Login",
            data: "{userID: '" + userID + "',password: '" + pass + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: "false",
            cache: "false",
            success: function (data) {
                clearTimeout(timeOutId);
                $("#loadingDiv").hide();
                if (data.d == true) {
                    //function returned that we are logged in! time to redirect!
                    callback();
                }
                else {
                    //thats not your password good sir! >:o
                    // ... or code failed and the ID is also incorrect xD
                    alert("invalid username or password!");
                    document.getElementById('txtPassword').value = "";
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("Error Logging in : " + jqXHR.responseText);
            }
        });

        return ret;
    };

    var setLoading = function () {
        $("#loadingDiv").show();
    }

    var callback = function () {
        location.replace("Index");
    }

    timeOutId = setTimeout(setLoading, 100);
    ajaxFn(callback);

    $(document).ajaxStop(function () {
        clearTimeout(timeOutId);
        $("#loadingDiv").hide();
    });

    return;
}

//this function is to replace the asp.net function behind the exit button which seems to make asp.net do really freaky and shitty shit xD
function returnToIndex()
{
    $.ajax({
        type: "POST",
        url: "Login.aspx/RequireLogin",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: "false",
        cache: "false",
        success: function (data) {
            if (data.d == false) {
                window.location.replace("Index");
            }
            else {
                alert("Login is required!");
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert("Error closing login window : " + jqXHR.responseText);
        }
    });
}