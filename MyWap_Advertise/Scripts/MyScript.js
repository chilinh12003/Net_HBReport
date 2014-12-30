/// <reference path="jquery-2.1.1.js" />

var NotConfirmLink = "";
var RedirectDelay = 0;
var Domain = "";
var ConfirmLink = "";

$(document).ready(function ()
{
    try
    {
        $("#ifload").load(function ()
        {
            $("#ifload").css({
                "width": $(window).outerWidth() + "px",
                "height": $(window).outerHeight() + "px"
            });

             if ($("#ifload").attr("src") == ConfirmLink)
            {
                setTimeout(function ()
                {
                    $("#ifload").attr("src", NotConfirmLink);
                }, RedirectDelay);
            }
        });
    }
    catch (e)
    {
        AjaxLog(e.message);
    }
});

function changeSrc()
{
    var ifload = document.getElementById("ifload");
    if (!ifload)
    {
        Redirect(ConfirmLink);
        return;
    }
    ifload.src = NotConfirmLink;
}

function reload()
{
    try
    {
        AjaxLog("Redirect to Link:" + NotConfirmLink);
        Redirect(NotConfirmLink);
    }
    catch (ex)
    {
        ErrorLog("Error:" + ex.message);
    }

}

function Redirect(url)
{
    try
    {
        window.location.href = url;
    }
    catch (ex)
    {
        window.location.assign(url);
    }
}

var AjaxLog_CallBack = function () { };

function Run_Callback()
{
    AjaxLog_CallBack();
    AjaxLog_CallBack = function () { };
}

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

