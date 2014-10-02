/// <reference path="jquery-2.1.1.min.js" />
var Domain = "http://localhost:8082/Advertise_Wap"
$(document).ready(function ()
{
    var mTimeout = setTimeout('reload()', 1000);
});

function reload()
{
    debugger;
    window.location.href = "http://wapgate.vinaphone.com.vn/MobileAds2014/reg/HBTPTT/CP/TRIEUPHUTT/TRIEUPHUTT/1";
}

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

function CreateParameterTime(str_URL)
{
    if (str_URL.indexOf("?", 0) > 0)
    {
        return str_URL + "&time=" + (new Date()).getTime();
    }
    else
    {
        return str_URL + "?time=" + (new Date()).getTime();
    }
}


//Hàm ajax
function AjaxFunction(URL_Get)
{
    var xmlHttp = CreateAjax();

    xmlHttp.onreadystatechange = function ()
    {
        if (xmlHttp.readyState == 4)
        {
            debugger;
            window.location.href = "http://wapgate.vinaphone.com.vn/MobileAds2014/reg/HBTPTT/CP/TRIEUPHUTT/TRIEUPHUTT/1";
        }
    }

    xmlHttp.open("GET", URL_Get, true);
    xmlHttp.send(null);
}