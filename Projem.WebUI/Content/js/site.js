function functAddCart(productID, quantity) {
    $.ajax({
        type: "POST",
        url: "/Home/AddCart",
        data: { "productID": productID, "quantity": quantity },
        success: function () { alert('eklendi') },
        error: function (e) { alert(e.responseText) },
    });
    $(".productCount").text(parseInt($(".productCount").text()) + 1)
}
function deleteCart(productID) {
    $.ajax({
        type: "POST",
        url: "/Home/DeleteCart",
        data: { "productID": productID },
        success: function () { location.href = '/Cart'; },
        error: function (e) { alert(e.responseText) },
    });
}
function updateCart(productID, quantity, sender, pm) {
    if (parseInt($(sender).parent().find(".cartDetailValue").val()) + parseInt(pm) != 0) {

        $.ajax({
            type: "POST",
            url: "/Home/UpdateCart",
            data: { "productID": productID, "quantity": parseInt($(sender).parent().find(".cartDetailValue").val()) + parseInt(pm) },
            success: function () { location.href = '/Cart'; },
            error: function (e) { alert(e.responseText) },
        });
    }
}
function functcheckoutAddress(sender) {
    $(".Address").val("");
    $(".City").val("");
    $(".District").val("");
    $(".Name").val("");
    if ($(sender).val() != "") {

        $.ajax({
            type: "GET",
            url: "/Checkout/GetAddress",
            data: { "addressID": $(sender).val() },
            success: function (data) {
                $(".Address").val(data.MemberAddress);
                $(".Address").attr("readonly", "readonly");
                $(".City").val(data.City);
                $(".City").attr("readonly", "readonly");
                $(".District").val(data.District);
                $(".District").attr("readonly", "readonly");
            },
            error: function (e) { alert(e.responseText) },
        });
        $(".Name").fadeOut();
    }
    else {
        $(".Address").removeAttr("readonly");
        $(".City").removeAttr("readonly");
        $(".District").removeAttr("readonly");
        $(".Name").fadeIn();
    }
}
function functAddFavorite(sender) {
    var productID = parseInt($(sender).siblings(".productID").val());
    $.ajax({
        type: "POST",
        url: "/Contact/addFavorite",
        //burada contact diyoruz cart ve hom ındex te nasl algılayacak
        data: { "productID": productID },
        success: function (data) {
            if (parseInt(data) == 0) {
                showModal("Favorilerden çıkarıldı.");
                $(sender).children().css("color", "#000");
            }
            else {
                showModal("Favorilere eklendi.");
                $(sender).children().css("color","red");
            }         
        },
        error: function (e) { alert(e.responseText) },
    });
}
function showModal(baslik) {
    $("#denemeModal").modal();
    $("#sonuc").text(baslik)
    setTimeout(function () { $("#denemeModal").modal('hide')},3000)
}
    
