(function ($) {
    "use strict";

    // Spinner
    var spinner = function () {
        setTimeout(function () {
            if ($('#spinner').length > 0) {
                $('#spinner').removeClass('show');
            }
        }, 1);
    };
    spinner();
    
    
    // Initiate the wowjs
    new WOW().init();


    // Sticky Navbar
    $(window).scroll(function () {
        if ($(this).scrollTop() > 300) {
            $('.sticky-top').css('top', '0px');
        } else {
            $('.sticky-top').css('top', '-100px');
        }
    });
    
    
    // Dropdown on mouse hover
    const $dropdown = $(".dropdown");
    const $dropdownToggle = $(".dropdown-toggle");
    const $dropdownMenu = $(".dropdown-menu");
    const showClass = "show";
    
    $(window).on("load resize", function() {
        if (this.matchMedia("(min-width: 992px)").matches) {
            $dropdown.hover(
            function() {
                const $this = $(this);
                $this.addClass(showClass);
                $this.find($dropdownToggle).attr("aria-expanded", "true");
                $this.find($dropdownMenu).addClass(showClass);
            },
            function() {
                const $this = $(this);
                $this.removeClass(showClass);
                $this.find($dropdownToggle).attr("aria-expanded", "false");
                $this.find($dropdownMenu).removeClass(showClass);
            }
            );
        } else {
            $dropdown.off("mouseenter mouseleave");
        }
    });
    
    
    // Back to top button
    $(window).scroll(function () {
        if ($(this).scrollTop() > 300) {
            $('.back-to-top').fadeIn('slow');
        } else {
            $('.back-to-top').fadeOut('slow');
        }
    });
    $('.back-to-top').click(function () {
        $('html, body').animate({scrollTop: 0}, 1500, 'easeInOutExpo');
        return false;
    });


    // Facts counter
    $('[data-toggle="counter-up"]').counterUp({
        delay: 10,
        time: 2000
    });


    // Date and time picker
    $('.date').datetimepicker({
        format: 'L'
    });
    $('.time').datetimepicker({
        format: 'LT'
    });


    // Testimonials carousel
    $(".testimonial-carousel").owlCarousel({
        autoplay: true,
        smartSpeed: 1000,
        center: true,
        margin: 25,
        dots: true,
        loop: true,
        nav : false,
        responsive: {
            0:{
                items:1
            },
            768:{
                items:2
            },
            992:{
                items:3
            }
        }
    });
    
})(jQuery);

//Customer
$(function () {
    loadUpdate();
});

$(document).on('click', '#tab-pane-1 #updatePersonal .btnUpdate', function () {
    var formData = new FormData();
    $('#updatePersonal input').each(function (x, y) {
        formData.append($(y).attr("name"), $(y).val());
    })
    $.ajax({
        type: 'POST',
        url: '/Customers/_PartialUpdatePersonal',
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            if (result.Success) {
                $('#insuranceModal .modal-dialog').removeClass("modal-lg");
                $('#insuranceModal .modal-dialog').addClass("modal-md");
                $('#insuranceModal button.btnInvoice').prop('hidden', true);
                $('#insuranceModal button.btnPrint').prop('hidden', true);
                showModal("Notification", "You have successfully updated.");
                loadUpdate();
            } else {
                $('#tab-pane-1').html(result);
                $('#ConfirmPassword').val($('#Password').val());
            }
        },
        error: function () {
            alert('Error');
        }
    });
});

$(document).on('click', '#tab-pane-1 .btnChangePassword', function () {
    $.ajax({
        url: '/Customers/_PartialChangePassword',
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#tab-pane-1').html(result);
        },
        error: function (e) {
            alert("Err");
            console.log(e);
        }
    });
});

$(document).on('click', '#tab-pane-1 #changePassword .btnChange', function () {
    var formData = new FormData();
    $('#changePassword input').each(function (x, y) {
        formData.append($(y).attr("name"), $(y).val());
    })
    $.ajax({
        type: 'POST',
        url: '/Customers/_PartialChangePassword',
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            if (result.Success) {
                $('#insuranceModal .modal-dialog').removeClass("modal-lg");
                $('#insuranceModal .modal-dialog').addClass("modal-md");
                $('#insuranceModal button.btnInvoice').prop('hidden', true);
                $('#insuranceModal button.btnPrint').prop('hidden', true);
                showModal("Notification", "You have successfully changed.");
                loadUpdate();
            } else {
                $('#tab-pane-1').html(result);
            }
        },
        error: function () {
            alert('Error');
        }
    });
});

$(document).on('click', '#tab-pane-1 #changePassword .btnBack', function () {
    loadUpdate();
});

function loadUpdate() {
    $.ajax({
        url: '/Customers/_PartialUpdatePersonal',
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#tab-pane-1').html(result);
            $('#ConfirmPassword').val($('#Password').val());
        },
        error: function (e) {
            console.log(e);
        }
    });
}

//Vehicle
function loadVehicleList() {
    $.ajax({
        url: '/Vehicles/_PartialVehicleList',
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#tab-pane-2').html(result);
            $('#vehicleList').DataTable();
        },
        error: function (e) {
            console.log(e);
        }
    });
}

$(function () {
    loadVehicleList();
});

$(document).on('click', '#tab-pane-2 .btnAddVehicle', function () {
    $.ajax({
        url: '/Vehicles/_PartialAddVehicle?brandID=' + null,
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#tab-pane-2').html(result);
            $('#addVehicle select').find('option:first').prop('disabled', true);
        },
        error: function (e) {
            console.log(e);
        }
    });
});

$(document).on('change', '#tab-pane-2 #addVehicle #VehicleBrandID', function () {
    var brandID = $(this).val();
    var typeID = $('#VehicleTypeID').val();
    var owner = $('#VehicleOwnerName').val();
    var bodyNum = $('#VehicleBodyNumber').val();
    var engineNum = $('#VehicleEngineNumber').val();
    var vehicleNum = $('#VehicleNumber').val();
    var seatNumber = $('#SeatNumber').val();
    $.ajax({
        url: '/Vehicles/_PartialAddVehicle?brandID=' + brandID,
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#tab-pane-2').html(result);
            $('#addVehicle select').find('option:first').prop('disabled', true);
            $('#VehicleBrandID').val(brandID);
            $('#VehicleTypeID').val(typeID);
            $('#VehicleOwnerName').val(owner);
            $('#VehicleBodyNumber').val(bodyNum);
            $('#VehicleEngineNumber').val(engineNum);
            $('#VehicleNumber').val(vehicleNum);
            $('#SeatNumber').val(seatNumber);
        },
        error: function (e) {
            console.log(e);
        }
    });
});

$(document).on('click', '#tab-pane-2 #addVehicle .btnAdd', function () {
    var brandID = $('#VehicleBrandID').val();
    var typeID = $('#VehicleTypeID').val();
    var modelID = $('#VehicleModelID').val();
    var seatNum = $('#SeatNumber').val();
    var image = document.getElementById("uploadFile").files[0];
    var formData = new FormData();
    $('#addVehicle input').each(function (x, y) {
        formData.append($(y).attr("name"), $(y).val());
    })
    $('#addVehicle select').each(function (x, y) {
        formData.append($(y).attr("name"), $(y).val());
    })
    var file = document.getElementById("uploadFile").files[0];
    formData.append("uploadFile", file);
    $.ajax({
        type: 'POST',
        url: '/Vehicles/_PartialAddVehicle?brandID=' + brandID,
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            if (result.Success) {
                $('#insuranceModal .modal-dialog').removeClass("modal-lg");
                $('#insuranceModal .modal-dialog').addClass("modal-md");
                $('#insuranceModal button.btnInvoice').prop('hidden', true);
                $('#insuranceModal button.btnPrint').prop('hidden', true);
                showModal("Notification", "You have successfully added.");
                loadVehicleList();
            } else {
                $('#tab-pane-2').html(result);
                $('#VehicleBrandID').val(brandID);
                if (brandID == null || brandID == "") {
                    $('#VehicleBrandID').closest('div').find('span').text('Brand is required');
                }
                if (typeID == null || typeID == "") {
                    $('#VehicleTypeID').closest('div').find('span').text('Vehicle Type is required');
                }
                if (modelID == null || modelID == "") {
                    $('#VehicleModelID').closest('div').find('span').text('Model is required');
                }
                if (seatNum == null || seatNum == "") {
                    $('#SeatNumber').closest('div').find('span').text('Seat Number is required');
                }
                if (image == undefined) {
                    $('#uploadFile').closest('div').find('span').text('Image is required');
                }
            }
        },
        error: function () {
            alert('Error');
        }
    });
});

$(document).on('click', '#tab-pane-2 #addVehicle .btnBack', function () {
    loadVehicleList();
});

$(document).on('click', '#tab-pane-2 #editVehicle .btnBack', function () {
    loadVehicleList();
});

var vehicleID;
$(document).on('click', '#tab-pane-2 .btnEditVehicle', function () {
    if ($(this).attr('vehicle-status') != 'Enable') {
        vehicleID = $(this).attr('id');
        $.ajax({
            url: '/Vehicles/_PartialEditVehicle?id=' + vehicleID + '&brandID=' + null,
            type: 'GET',
            cache: false,
            success: function (result) {
                $('#tab-pane-2').html(result);
                $('#editVehicle select').find('option:first').prop('disabled', true);
            },
            error: function (e) {
                console.log(e);
            }
        });
    } else {
        $('#insuranceModal .modal-dialog').removeClass("modal-lg");
        $('#insuranceModal .modal-dialog').addClass("modal-md");
        $('#insuranceModal button.btnInvoice').prop('hidden', true);
        $('#insuranceModal button.btnPrint').prop('hidden', true);
        showModal("Notification", "You cannot edit this vehicle.");
    }
});

$(document).on('change', '#tab-pane-2 #editVehicle #VehicleBrandID', function () {
    var brandID = $(this).val();
    $.ajax({
        url: '/Vehicles/_PartialEditVehicle?id=' + vehicleID + '&brandID=' + brandID,
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#tab-pane-2').html(result);
            $('#editVehicle select').find('option:first').prop('disabled', true);
            $('#VehicleBrandID').val(brandID);
        },
        error: function (e) {
            console.log(e);
        }
    });
});

$(document).on('click', '#tab-pane-2 #editVehicle .btnEdit', function () {
    var brandID = $('#VehicleBrandID').val();
    var image = document.getElementById("uploadFile").files[0];
    var formData = new FormData();
    $('#editVehicle input').each(function (x, y) {
        formData.append($(y).attr("name"), $(y).val());
    })
    $('#editVehicle select').each(function (x, y) {
        formData.append($(y).attr("name"), $(y).val());
    })
    var file = document.getElementById("uploadFile").files[0];
    formData.append("uploadFile", file);
    $.ajax({
        type: 'POST',
        url: '/Vehicles/_PartialEditVehicle?brandID=' + brandID,
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            if (result.Success) {
                $('#insuranceModal .modal-dialog').removeClass("modal-lg");
                $('#insuranceModal .modal-dialog').addClass("modal-md");
                $('#insuranceModal button.btnInvoice').prop('hidden', true);
                $('#insuranceModal button.btnPrint').prop('hidden', true);
                showModal("Notification", "You have successfully updated.");
                loadVehicleList();
            } else {
                $('#tab-pane-2').html(result);
                $('#VehicleBrandID').val(brandID);
            }
        },
        error: function () {
            alert('Error');
        }
    });
});

//Insurance
function loadOrderList() {
    $.ajax({
        url: '/OrderPolicies/_PartialOrderList',
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#tab-pane-3').html(result);
            $('#orderList').DataTable();
        },
        error: function (e) {
            console.log(e);
        }
    });
}

$(function () {
    loadOrderList();
});

var orderID;

$(document).on('click', '#tab-pane-3 .btnViewBill', function () {
    orderID = $(this).attr('id');
    $.ajax({
        url: '/OrderPolicies/_PartialOrderDetails?orderID=' + orderID,
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#insuranceModal .modal-dialog').removeClass("modal-md");
            $('#insuranceModal .modal-dialog').addClass("modal-lg");
            $('#insuranceModal button.btnInvoice').prop('hidden', false);
            $('#insuranceModal button.btnPrint').prop('hidden', true);
            /*chu y*/
            showModal("Insurance Details", result);
        },
        error: function (e) {
            console.log(e);
        }
    });
});

$(document).on('click', '#insuranceModal button.btnInvoice', function () {
    $.ajax({
        url: '/OrderPolicies/_PartialInvoice?orderID=' + orderID,
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#insuranceModal .modal-dialog').removeClass("modal-md");
            $('#insuranceModal .modal-dialog').addClass("modal-lg");
            $('#insuranceModal .modal-dialog .modal-footer').empty();
            $('#insuranceModal .modal-dialog .modal-footer').append("<button type='button' class='btn btn-default btnInvoice'>Invoice</button>");
            var url = "/OrderPolicies/PrintInvoice?orderID=" + orderID;
            $('#insuranceModal .modal-dialog .modal-footer').append("<button type='button' class='btn btn-default btnPrint' onclick=\"location.href='" + url + "';\">Print</button>");
            $('#insuranceModal .modal-dialog .modal-footer').append("<button type='button' class='btn btn-dark btnClose' onclick='closeModal();'>Close</button>");
            $('#insuranceModal button.btnInvoice').prop('hidden', true);
            $('#insuranceModal button.btnPrint').prop('hidden', false);
            showModal("Insurance Details", result);
        },
        error: function (e) {
            console.log(e);
        }
    });
});

//Claim
$(function () {
    loadClaimList();
});

$(document).on('click', '#tab-pane-4 .btnAddClaim', function () {
    $.ajax({
        url: '/Claims/_PartialAddClaim',
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#tab-pane-4').html(result);
            $('#tab-pane-4 select').find('option:first').prop('disabled', true);
        },
        error: function (e) {
            console.log(e);
        }
    });
});

$(document).on('click', '#tab-pane-4 #addClaim .btnAdd', function () {
    var formData = new FormData();
    $('#addClaim input').each(function (x, y) {
        formData.append($(y).attr("name"), $(y).val());
    })
    $('#addClaim select').each(function (x, y) {
        formData.append($(y).attr("name"), $(y).val());
    })
    $.ajax({
        type: 'POST',
        url: '/Claims/_PartialAddClaim',
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            if (result.Success) {
                $('#insuranceModal .modal-dialog').removeClass("modal-lg");
                $('#insuranceModal .modal-dialog').addClass("modal-md");
                $('#insuranceModal button.btnInvoice').prop('hidden', true);
                $('#insuranceModal button.btnPrint').prop('hidden', true);
                showModal("Notification", "You have successfully added.");
                $.ajax({
                    url: '/Claims/_PartialClaimList',
                    type: 'GET',
                    cache: false,
                    success: function (result) {
                        $('#tab-pane-4').html(result);
                    },
                    error: function (e) {
                        console.log(e);
                    }
                });
            } else {
                $('#tab-pane-4').html(result);
            }
        },
        error: function () {
            alert('Error');
        }
    });
});

$(document).on('click', '#tab-pane-4 #addClaim .btnBack', function () {
    loadClaimList();
});

function loadClaimList() {
    $.ajax({
        url: '/Claims/_PartialClaimList',
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#tab-pane-4').html(result);
            $('#claimList').DataTable();
        },
        error: function (e) {
            console.log(e);
        }
    });
}



var InsuranceID;

$(document).on('click', '.btnReadMore', function () {
    InsuranceID = $(this).attr('id');
    $.ajax({
        url: '/InsuranceTypes/_PartialInsurance?id=' + InsuranceID,
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#insuranceModal .modal-dialog').removeClass("modal-md");
            $('#insuranceModal .modal-dialog').addClass("modal-lg");
            /*chu y*/
            showModal("Insurance Types", result);
        },
        error: function (e) {
            console.log(e);
        }
    });
});

function showModal(header, body) {
    $('#insuranceModal .modal-dialog .modal-content .modal-header h4').text(header);
    $('#insuranceModal .modal-dialog .modal-content .modal-body').empty();
    $('#insuranceModal .modal-dialog .modal-content .modal-body').append(body);
    $("#insuranceModal").modal("show");

    $(document).on("click", "#insuranceModal button.btnClose", function (e) {
        $("#insuranceModal").modal("hide");
    });
}