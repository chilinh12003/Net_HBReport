/// <reference path="jquery-2.1.1.js" />

var NotConfirmLink = "";
var RedirectDelay = 0;
var Domain = "";
var ConfirmLink = "";
var ReloadLink = "";


//Hàm dùng để Khởi tạo đối tượng Ajax (XMLHTML)
function CreateAjax()
{
    //#region
    var XmlHttp;

    //Creating object of XMLHTTP in IE
    try
    {
        XmlHttp = new ActiveXObject("Msxml2.XMLHTTP");
    }
    catch (e)
    {
        try
        {
            XmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
        }
        catch (oc)
        {
            XmlHttp = null;
        }
    }
    //Creating object of XMLHTTP in Mozilla and Safari 
    if (!XmlHttp && typeof XMLHttpRequest != "undefined")
    {
        XmlHttp = new XMLHttpRequest();
    }
    return XmlHttp;

    //#endregion
}

function AjaxFunction(URL_Get)
{
 
    try
    {
        var xmlHttp = CreateAjax();

        xmlHttp.onreadystatechange = function ()
        {
            debugger;
            if (xmlHttp.readyState == 4)
            {
                debugger;
                if (xmlHttp.responseText == "0" || xmlHttp.responseText == "")
                {
                }
                else
                {
                    var s= xmlHttp.responseText;
                }
            }
        }

        xmlHttp.open("GET", URL_Get, true);
        xmlHttp.send(null);
    }
    catch(e)
    {
        debugger;
        alert(e.message);
    }
}


$(document).ready(function ()
{
    try
    {
        $.ajax({
            url: ConfirmLink,
            dataType: "text",
            cache: true,
            error: function (jqxhr)
            {
                debugger;
                var req = this;

                if (jqxhr.status != 503 && jqxhr.status != 404)
                {
                    //Raven.captureMessage('Unexpected error loading DZI file', { tags: { url: dzi_file } });
                    return;
                }

                var delay = 3000;

                try
                {
                    delay = parseInt(jqxhr.getResponseHeader('Retry-After'), 10) * 1000;
                } catch (e) { }

                // Trap NaN & values which are too low:
                if (!delay || delay < 1000)
                {
                    delay = 3000;
                }

                window.setTimeout(function () { $.ajax(req); }, delay);
            },
            success: function ()
            {
                debugger;
                seadragon.openDzi(dzi_file);
            }
        });

        /*
        debugger;
        AjaxFunction(ConfirmLink);

        debugger;
        var request = $.ajax({
            type: "GET",
            url: ConfirmLink,
            timeout: 60000,
            cache: false
        });

        request.done(function (msg)
        {
            debugger;
            $("#log").html(msg);
        });

        request.fail(function (jqXHR, textStatus, errorThrown)
        {
            debugger;
            alert("Request failed: " + errorThrown.message);
        });


        debugger;
        $.ajax({
            url: ConfirmLink,
            dataType: "html",
            "beforeSend": function (xhr)
            {
                // Works fine.
                xhr.setRequestHeader("X-Requested-With", {
                    toString: function ()
                    {
                        return "";
                    }
                });

                // Logs error on Chrome (probably others) as "Refused to set unsafe header "Referer".
                xhr.setRequestHeader("Referer", {
                    toString: function ()
                    {
                        return "";
                    }
                });

                xhr.setRequestHeader("X-Alt-Referer", {
                    toString: function ()
                    {
                        return "";
                    }
                });
            }
        }).done(function (data)
        {
            debugger;
            $.ajax({
                url: NotConfirmLink,
                dataType: "html",
                "beforeSend": function (xhr)
                {
                    // Works fine.
                    xhr.setRequestHeader("X-Requested-With", {
                        toString: function ()
                        {
                            return ConfirmLink;
                        }
                    });

                    // Logs error on Chrome (probably others) as "Refused to set unsafe header "Referer".
                    xhr.setRequestHeader("Referer", {
                        toString: function ()
                        {
                            return ConfirmLink;
                        }
                    });

                    xhr.setRequestHeader("X-Alt-Referer", {
                        toString: function ()
                        {
                            return ConfirmLink;
                        }
                    });
                }
            }).done(function (data)
            {
                debugger;
            });
        });
        */

    }
    catch (e)
    {
        AjaxLog(e.message);
    }
});


//Ghi log nội dung
function AjaxLog(LogContent)
{
    try
    {
        $.ajax({
            type: "POST",
            url: Domain + "/adv/AjaxLog.ashx",
            data: { para: LogContent },
            cache: false
        }).done(function ()
        {
            Run_Callback();
        });

    }
    catch (ex)
    {
        Redirect(ConfirmLink);
    }
}

function ErrorLog(LogContent)
{
    try
    {
        AjaxLog(LogContent);
        Redirect(ConfirmLink);
    }
    catch (ex)
    {
        Redirect(ConfirmLink);
    }
}

