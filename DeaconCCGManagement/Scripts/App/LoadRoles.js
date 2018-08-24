//
//This uses Ajax to pass the selected email to an action method; returns 
//user's roles and displays them.
//


var loaderImg = '<img src="~/Content/Images/loader.white.gif" id="loaderImg" alt="Loading" style="display: none;" />';

// Gets the selected email in the dropdownlist and passes email to action 
// method using the AJAX wrapper method 'post'
function getEmailAndEchoUserRoles() {
    $.post('@Url.Action("UserRoles", "Auth")',
        { email: $("#selectEmail").val() })
        .done(function (data) {
            { $("#userroles").html(data); };
        });
};


//$(‘#loaderImg’).toggle(‘slow’);

//setTimeout(3000);

//setTimeout(function () { $("#userroles").html(data); }, 3000);
//{ $("#userroles").html(data); };

//function showLoaderImg() {
//    $("#loaderImg").show();
//};

//function hideLoaderImg() {
//    $("#loaderImg").hide();
//};

//function emptyDiv() {
//    $("#userroles").empty();
//};
$(document).ready(function () {

    $(function () {
        //getEmailAndEchoUserRoles();
        //$("#loaderImg").hide();
        //$("#userroles").empty();
        //$("#loaderImg").show();


        getEmailAndEchoUserRoles();


        //$("#loaderImg").hide();

        $("#selectEmail").change(function () {
            //getEmailAndEchoUserRoles();

            //$("#userroles").html(loaderImg);

            //$("#userroles").empty();
            //$("#loaderImg").show();

            getEmailAndEchoUserRoles();
            //$("#loaderImg").hide();

            //showLoaderImg();

            //$("#userroles").empty();

            //$("#loaderImg").show();
            //emptyDiv();
            //addImg();

            //hideLoaderImg();
        });
    });

    //$(document).ajaxStop(function () { $("#loaderImg").show() });

    // $(document).ajaxStop(function() { $("#loaderImg").hide() });

    $("#test").click(function () {
        $("#loaderImg").hide();
    });

});
