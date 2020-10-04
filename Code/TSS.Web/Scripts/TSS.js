//general code to load a div from a page into a div of the current page
function LoadPageDivIntoDiv(pageName, PageDiv, CurrentDiv, parameters, callback, Async) {
    var ret = false;
    var mode = true;
    var timeOutId = 0;

    if (Async != null)
        mode = (Async == true);

    if (parameters == null)
        parameters = "{}";

    function LoadPage() {
        $.ajax({
            type: "GET",
            url: pageName,
            cache: false,
            async: mode,
            datatype: "html",
            data: parameters,
            timeout: 30000,
            success: function(data) {
                let div = $(data).find(PageDiv);
                let content = div.html();
                $(CurrentDiv).html(content);
                if (callback) {
                    callback();
                }
            },
            error: function(jqXHR, textStatus, errorThrown) {
                alert(textStatus);
                alert(errorThrown);
                alert("failed!");
            }
        });
        return true;
    };
    var setLoading = function() {
        var loadingdiv = document.getElementById("loadingDiv");
        if (loadingdiv)
            loadingdiv.hidden = false;
    };

    
    $(document).ajaxStart(function () {
        //time out doesn't work on sync ajax calls as it blocks the thread.
        //however, isn't this the point of timers & async calls ...?
        //anyway, force the loading to show up..
        //timeOutId = setTimeout(setLoading, 1);
        setLoading();
    });

    //and clear the image when we are done
    $(document).ajaxStop(function () {
        clearTimeout(timeOutId);
        var loadingdiv = document.getElementById("loadingDiv");
        if (loadingdiv)
            loadingdiv.hidden = true;
    });

    ret = LoadPage();
    return ret;
}

//calls upon the ASP.NET function that will push the changes to the Page which is keeping track of all changes
function PushNewPropertyValue(PageName, PropertyName, value, Async) {
    var mode = true;
    var ret = false;
    if (Async != null)
        mode = (Async == true);
    var ajaxFn = function() {
        $.ajax({
            type: "POST",
            url: PageName + "/NewPropertyValue",
            data: "{PropertyName:'" + PropertyName + "',value:'" + value + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: mode,
            cache: "false",
            success: function(data) {
                if (data.d == true) {
                    ret = true;
                    return true;
                } else {
                    ret = false;
                    return false;
                }
            },
            error: function(jqXHR, textStatus, errorThrown) {
                alert("Error saving changes of " + PropertyName + ", error : " + jqXHR.responseText);
            }
        });
    };

    ajaxFn();
    return ret;
}