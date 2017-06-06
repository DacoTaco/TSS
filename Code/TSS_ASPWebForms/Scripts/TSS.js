//general code to load a div from a page into a div of the current page
function LoadPageDivIntoDiv(pageName, PageDiv, CurrentDiv, parameters, callback,Async) {
    var ret = false;
    var mode = true;
    var timeOutId;

    if (Async != null)
        mode = (Async == true);

    if (parameters == null)
        parameters = "{}";

    var ajaxFn = function () {
        $.ajax({
            type: "GET",
            url: pageName,
            async: mode,
            cache: false,
            datatype: 'html',
            data: parameters,
            timeout: 30000,
            success: function (data) {
                // data is ur summary
                //var div = $(PageDiv, $(data)).addClass('done').html();
                var div = $(data).find(PageDiv);
                var content = div.html();
                $(CurrentDiv).html(content);

                // data is our summary
                //var div = $(PageDiv).html($(data).find(PageDiv).html());

                //google said to set data like this. but this seems to kill some ASP related shit?
                //$(CurrentDiv).html(div);
                //document.getElementById('TaskView').innerHTML = div;

                clearTimeout(timeOutId);
                var loadingdiv = document.getElementById("loadingDiv");
                if (loadingdiv)
                    loadingdiv.hidden = true;

                if (callback) {
                    callback();
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("failed!");
            }

        });
        return true;
    }
    var setLoading = function () {
        var loadingdiv = document.getElementById("loadingDiv");
        if (loadingdiv)
            loadingdiv.hidden = false;
    }

    timeOutId = setTimeout(setLoading, 200);
    ret = ajaxFn();
    $(document).ajaxStop(function () {
        clearTimeout(timeOutId);
        var loadingdiv = document.getElementById("loadingDiv");
        if (loadingdiv)
            loadingdiv.hidden = true;
    });

    return ret;
}

//calls upon the ASP.NET function that will push the changes to the Page which is keeping track of all changes
function PushNewPropertyValue(PageName,PropertyName, value, Async) {
    var mode = true;
    var ret = false;
    if (Async != null)
        mode = (Async == true);
    var ajaxFn = function () {
        $.ajax({
            type: "POST",
            url: PageName + "/NewPropertyValue",
            data: "{PropertyName:'" + PropertyName + "',value:'" + value + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: mode,
            cache: "false",
            success: function (data) {
                if (data.d == true) {
                    ret = true;
                    return true;
                }
                else {
                    ret = false;
                    return false;
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("Error saving changes of " + PropertyName + ", error : " + jqXHR.responseText);
            }
        });
    }

    ajaxFn();
    return ret;
}

