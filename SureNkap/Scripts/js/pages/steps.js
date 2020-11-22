$(".tab-wizard").steps({
    headerTag: "h6"
    , bodyTag: "section"
    , transitionEffect: "none"
    , titleTemplate: '<span class="step">#index#</span> #title#'
    , labels: {
        finish: "Enregistrer"
    }
    , onFinished: function (event, currentIndex) {
       swal("Your Order Submitted!", "Sed dignissim lacinia nunc. Curabitur tortor. Pellentesque nibh. Aenean quam. In scelerisque sem at dolor. Maecenas mattis. Sed convallis tristique sem. Proin ut ligula vel nunc egestas porttitor.");
            
    }
});


var form = $(".validation-wizard").show();

$(".validation-wizard").steps({
    headerTag: "h6"
    , bodyTag: "section"
    , transitionEffect: "none"
    , titleTemplate: '<span class="step">#index#</span> #title#'
    , labels: {
        finish: "Enregistrer",
        next: "Suivant",
        previous: "Precedent"
    }
    , onStepChanging: function (event, currentIndex, newIndex) {
        if (newIndex > currentIndex && currentIndex == 2) {
            if ($('#lat').val() == "" || $('#long').val() == "") {
                swal("Position Inconnue!", "Cliquer sur le button Géolocaliser maintenant pour donner votre position", "warning");
                return false;
            }
        }
        return currentIndex > newIndex || !(3 === newIndex && Number($("#age-2").val()) < 18) && (currentIndex < newIndex && (form.find(".body:eq(" + newIndex + ") label.error").remove(), form.find(".body:eq(" + newIndex + ") .error").removeClass("error")), form.validate().settings.ignore = ":disabled,:hidden", form.valid())
    }
    , onFinishing: function (event, currentIndex) {
        return form.validate().settings.ignore = ":disabled", form.valid()
    }
    , onFinished: function (event, currentIndex) {
        var check = $('#basic_checkbox_1').is(":checked");
        if (check) {
            swal({
                title: "Enregistrer ?",
                text: "Vous allez enregistrer vos informations. Certaines informations ne pourront plus être accessibles",
                type: "warning",
                showCancelButton: true,
                closeOnConfirm: false,
                animation: "slide-from-top",
                showLoaderOnConfirm: true
            }, function (confirm) {
                if (confirm) {
                    var formData = new FormData();
                    formData.append("firstname", $("#firstname").val());
                    formData.append("rccm", $("#rccm").val());
                    formData.append("email", $("#email").val());
                    formData.append("password", $("#password").val());
                    formData.append("contact", $("#contact").val());
                    formData.append("address", $("#address").val());
                    formData.append("manager", $("#manager").val());
                    formData.append("mcontact", $("#mcontact").val());
                    formData.append("country", $("#country").val());
                    formData.append("city", $("#city").val());
                    formData.append("currency", $("#currency").val());
                    formData.append("timezone", $("#timezone").val());
                    formData.append("desc", $("#desc").val());
                    formData.append("lat", $("#lat").val());
                    formData.append("long", $("#long").val());
                    $.ajax({
                        url:"/Admin/CreatePharmacie",
                        type:"POST",
                        dataType:"json",
                        contentType:false,
                        processData:false,
                        data:formData,
                        success: function (result) {
                            if (result.success) {
                                swal(unescape("Enregistr%E9 avec succ%E8s"), unescape("Succ%E8ss"), "success");
                                setTimeout(function () { location.href = "/admin/index/" + result.id },2000);
                            }
                            else {
                                swal("Echec", result.message, "error");
                            }
                        },
                        error: function (err) {
                            swal("Erreur", err.responseText, "error");
                        }
                    });
                }
            });
            
        }
        else {
            swal("Conditions d'utilisation", "Vous devez accepter les conditions d'utilisation avant de continuer", "warning");
        }
         
    }
}), $(".validation-wizard").validate({
    ignore: "input[type=hidden]"
    , errorClass: "text-danger"
    , successClass: "text-success"
    , highlight: function (element, errorClass) {
        $(element).removeClass(errorClass)
    }
    , unhighlight: function (element, errorClass) {
        $(element).removeClass(errorClass)
    }
    , errorPlacement: function (error, element) {
        error.insertAfter(element)
    }
    , rules: {
        email: {
            email: !0
        }
    }
})