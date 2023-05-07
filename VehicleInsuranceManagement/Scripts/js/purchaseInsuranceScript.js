// Declare some variable
var c; // coefficient of vehicle
var id; // vehicle id of selected vehicle
var NoY; // number of years of insurance
var oldNoY; // number of years of insurance before change
var insuranceID; // insurance id of selected insurance
var oldInsuranceID; // insurance id of selected insurance before change
var insuranceStartDate; //start date of insurance
var insuranceEndDate; //end date of insurance
var insurancePrice; // price of insurance
var insuranceTotalPrice; // total order amount after multiplier by type of payment
var paymentType; // payment type selected
var maxNumberOfPayment; // the maximum number of payments a customer can choose
var numberOfPayment; // the number of payments a customer choose
var amountPerPayment; // the amount per payment

// Scroll to the top of the order form
function goToByScroll(id) {
    // Remove "link" from the ID
    id = id.replace("link", "");
    // Scroll
    $('html,body').animate({
        scrollTop: $("#" + id).offset().top
    }, 'fast');
}

// Load Estimate form
$(function () {
    $.ajax({
        url: '/PurchaseInsuranceProcesses/_PartialEstimate',
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#orderForm').html(result);
            $('#estimate button').prop('disabled', true);
        },
        error: function (e) {
            console.log(e);
        }
    });
});

// Estimating the cost of insurance according to the vehicle chosen by the customer
$(document).on("change", "#estimate", function () {
    $('#estimate button').prop('disabled', false);
    c = $('#vehicle').val();
    id = $("#vehicle").children(":selected").attr("id");

    // Load Estimate table of insurances cost
    $.ajax({
        url: '/PurchaseInsuranceProcesses/_PartialEstimateTable?c=' + c + '',
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#estimateTable').html(result);

            // Load selected vehicle details
            $.ajax({
                url: '/PurchaseInsuranceProcesses/_PartialEstimateVehicleDetails?vehicleID=' + id + '',
                type: "GET",
                cache: false,
                success: function (result1) {
                    $('#estimateVehicleDetails').html(result1);
                },
                error: function (e) {
                    console.log(e);
                }
            });
        },
        error: function (e) {
            console.log(e);
        }
    });
});

// Load Insurance Information form with VehicleID of selected vehicle from Estimate form
$(document).on("click", "#estimate button", function () {
    $('#purchaseInsurance button[data-bs-target="#tab-pane-1"]').removeClass('active');
    $('#purchaseInsurance button[data-bs-target="#tab-pane-2"]').addClass('active');

    goToByScroll("purchaseInsurance");

    $.ajax({
        url: '/PurchaseInsuranceProcesses/_PartialInsuranceInfo?vehicleID=' + id + '',
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#orderForm').html(result);
            $('#insuranceInfo button.btnContinue').prop('disabled', true);

            // Load javascript of Insurance Information form 
            loadInsuranceInfoScript();

            // Load the insurance information if it is not the first time click the "Continue" button from the Estimate form
            loadInsuranceInfoAgain();
        },
        error: function (e) {
            console.log(e);
        }
    });
});

// Load the insurance information
function loadInsuranceInfoAgain() {
    if (NoY == 1) {
        $("input[name=insuranceYear][value=1]").prop("checked", true);
    } else if (NoY == 2) {
        $("input[name=insuranceYear][value=2]").prop("checked", true);
    }

    if (insuranceStartDate != undefined) {
        $('#date1').datetimepicker('date', moment(insuranceStartDate.toString(), 'MM/DD/YYYY'));
    }

    if (insuranceID != undefined) {
        $('#insuranceInfo button.btnContinue').prop('disabled', false);
        $('#insurance #' + insuranceID).prop('selected', true);
        calculateInsurancePrice();
    }
}

// Format currency
function formatCurrency(total) {
    var neg = false;
    if (total < 0) {
        neg = true;
        total = Math.abs(total);
    }
    return (neg ? "-$" : '$') + parseFloat(total, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();
}

// Script for Insurance Information form
function loadInsuranceInfoScript() {
    $(function () {
        // Set default start date of "Duration"
        var today = new Date();
        today.setDate(today.getDate() + 1);
        var month = today.getMonth() + 1;
        var day = today.getDate();
        var outputToday = (month < 10 ? '0' : '') + month + '/' +
            (day < 10 ? '0' : '') + day + '/' +
            today.getFullYear();

        if (insuranceStartDate == undefined) {
            $('#date1').datetimepicker('date', moment(outputToday, 'MM/DD/YYYY'));
        }

        // Show datetimepicker when focus in Duration's textbox
        $("#insuranceInfo div.date input").click(function () {
            $('#insuranceInfo div#date1 input').removeAttr('disabled');
            $(this).closest('div.date').find('.bootstrap-datetimepicker-widget.dropdown-menu').css('display', 'block');
        });

        // Hide datetimepicker when out of focus in Duration's textbox
        $("#insuranceInfo div.date input").blur(function () {
            $(this).closest('div.date').find('.bootstrap-datetimepicker-widget.dropdown-menu').css('display', 'none');
        });

        // Disable Duration's textbox when customer enter
        $('#insuranceInfo div.date input').keydown(function (e) {
            $('#insuranceInfo div#date1 input').attr('disabled', 'disabled');
            $('#insuranceInfo div#date1 input').removeAttr('disabled');
        });

    });

    // Set end date of "Duration"
    function getInsuranceEndDate(NoY) {
        var from = $('#insuranceInfo div#date1 input').val().split("/");
        var today = new Date(from[2], from[0] - 1, from[1]);
        today.setDate(today.getDate() - 1);
        var month = today.getMonth() + 1;
        var day = today.getDate();
        if (NoY == 1) {
            today.setFullYear(today.getFullYear() + 1);
        }
        else if (NoY == 2) {
            today.setFullYear(today.getFullYear() + 2);
        }
        var outputTodayNextYear = (month < 10 ? '0' : '') + month + '/' +
            (day < 10 ? '0' : '') + day + '/' +
            today.getFullYear();

        $('#date2').datetimepicker('date', moment(outputTodayNextYear, 'MM/DD/YYYY'));
        insuranceEndDate = $('#insuranceInfo div#date2 input').val();
    }

    // Change start and end date of "Duration"
    $('#insuranceInfo div#date1 input').on('input', function (e) {
        insuranceStartDate = $('#insuranceInfo div#date1 input').val();
        NoY = $('input[name="insuranceYear"]:checked').val();
        getInsuranceEndDate(NoY);
        $(this).closest('div.date').find('.bootstrap-datetimepicker-widget.dropdown-menu').css('display', 'none');
    });

    // Change number of years of insurance and the insurance price
    $('input[type=radio][name=insuranceYear]').change(function () {
        oldNoY = NoY;
        NoY = $('input[name="insuranceYear"]:checked').val();
        getInsuranceEndDate(this.value);

        if (insuranceID != undefined) {
            calculateInsurancePrice();
        }
    });

    // Change the insurance and the insurance price
    $('#insurance').on('change', function () {
        oldInsuranceID = insuranceID;
        $('#insuranceInfo button.btnContinue').prop('disabled', false);
        insuranceID = $("#insurance").children(":selected").attr("id");

        calculateInsurancePrice();
    });
}

// Calculate the price of insurance
function calculateInsurancePrice() {
    insurancePrice = parseFloat($('#insurance').val());

    insurancePrice = (insurancePrice + insurancePrice * parseFloat(c)) * NoY;

    $('#contentOrder div.insurancePrice div.content span').text(formatCurrency(insurancePrice));
}

// Turn back Estimate form from Insurance Information form
$(document).on("click", "#insuranceInfo button.btnBack", function () {
    $('#purchaseInsurance button[data-bs-target="#tab-pane-2"]').removeClass('active');
    $('#purchaseInsurance button[data-bs-target="#tab-pane-1"]').addClass('active');

    // Load Estimate form
    $.ajax({
        url: '/PurchaseInsuranceProcesses/_PartialEstimate',
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#orderForm').html(result);
            $("#vehicle").val(c);

            // Load Estimate table of insurances cost of selected vehicle
            $.ajax({
                url: '/PurchaseInsuranceProcesses/_PartialEstimateTable?c=' + c + '',
                type: 'GET',
                cache: false,
                success: function (result) {
                    $('#estimateTable').html(result);

                    // Load vehicle details of selected vehicle
                    $.ajax({
                        url: '/PurchaseInsuranceProcesses/_PartialEstimateVehicleDetails?vehicleID=' + id + '',
                        type: "GET",
                        cache: false,
                        success: function (result1) {
                            $('#estimateVehicleDetails').html(result1);
                        },
                        error: function (e) {
                            console.log(e);
                        }
                    });
                },
                error: function (e) {
                    console.log(e);
                }
            });
        },
        error: function (e) {
            console.log(e);
        }
    });
});

// Load Payment Information form with VehicleID of selected vehicle from Estimate form and InsuranceID of selected insurance 
// from Insurance Information form
$(document).on("click", "#insuranceInfo button.btnContinue", function () {
    $('#purchaseInsurance button[data-bs-target="#tab-pane-2"]').removeClass('active');
    $('#purchaseInsurance button[data-bs-target="#tab-pane-3"]').addClass('active');
    goToByScroll("purchaseInsurance");
    $.ajax({
        url: '/PurchaseInsuranceProcesses/_PartialPaymentInfo?vehicleID=' + id + '&insuranceID=' + insuranceID + '',
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#orderForm').html(result);
            $('#paymentInfo button.btnContinue').prop('disabled', true);
            $('#paymentInfo .btn-minus').prop('disabled', true);
            $('#paymentInfo .btn-plus').prop('disabled', true);
            $('#paymentInfo div.amountPerPayment').prop('hidden', true);
            if (NoY == 1) {
                $("#paymentType #annual").prop('hidden', true);
            }

            if (insurancePrice * 1.1 < (75 * 2)) {
                $("#paymentType #monthly").prop('disabled', true);
                $("#paymentType #quarterly").prop('disabled', true);
                $("#paymentType #annual").prop('disabled', true);
            } else if (insurancePrice * 1.05 < (225 * 2)) {
                $("#paymentType #quarterly").prop('disabled', true);
                $("#paymentType #annual").prop('disabled', true);
            } else if (insurancePrice * 1.025 < (900 * 2)) {
                $("#paymentType #annual").prop('disabled', true);
            }

            $('#contentOrder div.insurancePrice div.content span').text(formatCurrency(insurancePrice));

            loadPaymentInfoScript();

            loadPaymentInfoAgain();
        },
        error: function (e) {
            console.log(e);
        }
    });
});

function loadPaymentInfoScript() {
    // Load the payment information if show payment form again
    function loadPaymentInfoAgain() {
        if (paymentType != undefined) {
            $('#paymentInfo button.btnContinue').prop('disabled', false);
            $('#paymentType #' + paymentType).prop('selected', true);
            paymentInfo();
            if (paymentType != "fully") {
                if (oldInsuranceID != undefined && oldInsuranceID != insuranceID) {
                    numberOfPayment = maxNumberOfPayment;
                    $('#paymentInfo .quantity').val(numberOfPayment);
                }
                if (oldNoY != undefined && oldNoY != NoY) {
                    numberOfPayment = maxNumberOfPayment;
                    $('#paymentInfo .quantity').val(numberOfPayment);
                }
            }
        }

        if (numberOfPayment != undefined) {
            $('#paymentInfo .quantity').val(numberOfPayment);
            calculateAmountPerPayment();
        }
    }

    loadPaymentInfoAgain();

    function enableElement() {
        $('#paymentInfo .btn-minus').prop('disabled', false);
        $('#paymentInfo .btn-plus').prop('disabled', false);
        $('#paymentInfo div.amountPerPayment').prop('hidden', false);
    }

    function disableElement() {
        $('#paymentInfo .btn-minus').prop('disabled', true);
        $('#paymentInfo .btn-plus').prop('disabled', true);
        $('#paymentInfo div.amountPerPayment').prop('hidden', true);
    }

    // Change number of payment that customer can choose and amount per payment when change payment type
    $('#paymentType').on('change', function () {
        paymentInfo();
        if (paymentType != "fully") {
            numberOfPayment = maxNumberOfPayment;
            $('#paymentInfo .quantity').val(numberOfPayment);
        }
        calculateAmountPerPayment();
    });

    // Payment infomation related to payment type
    function paymentInfo() {
        paymentType = $('#paymentType').children(":selected").attr('id');

        $('#paymentInfo button.btnContinue').prop('disabled', false);

        var percent = parseFloat($('#paymentType').val());

        var minMoney = parseFloat($('#paymentType').children(":selected").attr('min-money'));

        var max;

        // Total price of insurance after adding by type of payment
        insuranceTotalPrice = (insurancePrice + insurancePrice * percent);
        $('#contentOrder div.totalPrice div.content span').text(formatCurrency(insuranceTotalPrice));

        // Maximum number of payment depend on payment type
        if (paymentType == "fully") {
            numberOfPayment = 1;
            disableElement();
            $('#paymentInfo .quantity').val(1);
        } else {
            if (paymentType == "monthly") {
                if (NoY == 1) {
                    max = 12;
                } else if (NoY == 2) {
                    max = 24;
                }
            } else if (paymentType == "quarterly") {
                if (NoY == 1) {
                    max = 4;
                } else if (NoY == 2) {
                    max = 8;
                }
            } else if (paymentType == "annual") {
                max = 2;
            }

            enableElement();

            // Calculate max number of payment base on total price
            maxNumberOfPayment = Math.floor(insuranceTotalPrice / minMoney);
            if (maxNumberOfPayment > max) {
                maxNumberOfPayment = max;
            }

            $('#paymentInfo .quantity').attr('max', maxNumberOfPayment);
        }

        if (maxNumberOfPayment == 2) {
            disableElement();
            $('#paymentInfo div.amountPerPayment').prop('hidden', false);
        }
    }

    // Calculate amount per payment
    function calculateAmountPerPayment() {
        amountPerPayment = insuranceTotalPrice / numberOfPayment;
        $('#paymentInfo div.amountPerPayment div.amountPerPaymentContent').text(formatCurrency(amountPerPayment));
    }

    // Increase or descrease number of payment when click plus or minus button
    $('.btn-plus, .btn-minus').on('click', function (e) {
        e.preventDefault();
        const isNegative = $(e.target).closest('.btn-minus').is('.btn-minus');
        const input = $(e.target).closest('.input-group').find('input');
        if (input.is('input')) {
            input[0][isNegative ? 'stepDown' : 'stepUp']()
        }
    });

    $('.btn-plus').on('click', function (e) {
        if (numberOfPayment == maxNumberOfPayment) {
            $('#insuranceModal button.btnHome').prop('hidden', true);
            $('#insuranceModal button.btnPrint').prop('hidden', true);
            $('#insuranceModal .modal-dialog').addClass('modal-md');
            showModal("Warning", "Number of payment is from 2 to " + maxNumberOfPayment);
        }
        numberOfPayment = $('#paymentInfo .quantity').val();
        calculateAmountPerPayment();
    });

    $('.btn-minus').on('click', function (e) {
        if (numberOfPayment == 2) {
            $('#insuranceModal button.btnHome').prop('hidden', true);
            $('#insuranceModal button.btnPrint').prop('hidden', true);
            $('#insuranceModal .modal-dialog').addClass('modal-md');
            showModal("Warning", "Number of payment is from 2 to " + maxNumberOfPayment);
        }
        numberOfPayment = $('#paymentInfo .quantity').val();
        calculateAmountPerPayment();
    });
}

// Turn back Insurance Information form from Payment Information form
$(document).on("click", "#paymentInfo button.btnBack", function () {
    oldInsuranceID = insuranceID;
    oldNoY = NoY;
    $('#purchaseInsurance button[data-bs-target="#tab-pane-3"]').removeClass('active');
    $('#purchaseInsurance button[data-bs-target="#tab-pane-2"]').addClass('active');
    $.ajax({
        url: '/PurchaseInsuranceProcesses/_PartialInsuranceInfo?vehicleID=' + id + '',
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#orderForm').html(result);

            loadInsuranceInfoScript();

            loadInsuranceInfoAgain();
        },
        error: function (e) {
            console.log(e);
        }
    });
});

// Load Order Summary form with VehicleID of selected vehicle from Estimate form and InsuranceID of selected insurance 
// from Payment Information form
$(document).on("click", "#paymentInfo button.btnContinue", function () {
    oldInsuranceID = insuranceID;
    oldNoY = NoY;
    $('#purchaseInsurance button[data-bs-target="#tab-pane-3"]').removeClass('active');
    $('#purchaseInsurance button[data-bs-target="#tab-pane-4"]').addClass('active');

    goToByScroll("purchaseInsurance");

    $.ajax({
        url: '/PurchaseInsuranceProcesses/_PartialOrderSummary?vehicleID=' + id + '&insuranceID=' + insuranceID + '',
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#orderForm').html(result);

            // Set text for some order information for customer to view
            $('#orderSummary div.insuranceDate div.content span').text(insuranceStartDate + ' - ' + insuranceEndDate);
            $('#orderSummary div.totalPrice div.content span').text(formatCurrency(insuranceTotalPrice));
            $('#orderSummary div.paymentType div.content span').text(paymentType[0].toUpperCase() + paymentType.substring(1));

            // Set value about insurance for some hidden input to insert to database
            $('#orderSummary #PolicyDate').val(formatDate(insuranceStartDate));
            $('#orderSummary #PolicyDuration').val(NoY);
            $('#orderSummary #PaymentType').val(paymentType);

            calculatePaymentSchedule(paymentType);

            // Set value about payment to insert to database
            $('#orderSummary .amountPerPayment').val(amountPerPayment);
            $('#orderSummary .count').val(numberOfPayment);
        },
        error: function (e) {
            console.log(e);
        }
    });
});

// Format date
function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [year, month, day].join('-');
}

// Calculate payment schedule
function calculatePaymentSchedule(p) {
    var plusmonth
    if (p == "fully") {
        plusmonth = 0
    } else if (p == "monthly") {
        plusmonth = 1;
    } else if (p == "quarterly") {
        plusmonth = 3;
    } else if (p == "annual") {
        plusmonth = 12;
    }
    for (i = 0; i < numberOfPayment; i++) {
        var date = new Date(insuranceStartDate);
        date.setMonth(date.getMonth() + i * plusmonth);
        var month = date.getMonth() + 1;
        var day = date.getDate();
        var outputToday = (month < 10 ? '0' : '') + month + '/' +
            (day < 10 ? '0' : '') + day + '/' +
            date.getFullYear();
        $('#orderSummary #billSchedule').append("<input type='hidden' name='Date-" + i + "' value='" + formatDate(outputToday) + "' />")
        $('#orderSummary table tbody').append("<tr><td class='col-6'>" + outputToday + "</td><td class='col-6'>" + formatCurrency(amountPerPayment) + "</td></tr>")
    }
}

// Send order summary form data to _PartialOrderSummary action of PurchaseInsuranceProcesses controller to save into database and show 
// massage for customer
$(document).on("click", "#orderSummary button.btnBuy", function (e) {
    let myForm = $('#orderSummary');
    $.ajax({
        async: false,
        cache: false,
        type: 'POST',
        data: myForm.serialize(),
        url: '/PurchaseInsuranceProcesses/_PartialOrderSummary',
        success: function (result) {
            if (result.Success) {
                $('#orderSummary button').prop('disabled', true);
                $('#insuranceModal button.btnHome').prop('hidden', true);
                $('#insuranceModal button.btnPrint').prop('hidden', false);
                $.ajax({
                    url: '/PurchaseInsuranceProcesses/_PartialInvoice',
                    type: 'GET',
                    cache: false,
                    success: function (result) {
                        $('#insuranceModal .modal-dialog').addClass('modal-lg');
                        showModal("", result);
                    },
                    error: function (e) {
                        console.log(e);
                    }
                });
            } else {
                $('#insuranceModal .modal-dialog').addClass('modal-md');
                showModal("Error", "There was an error making your request, please try again.");
                //location.href = '@Url.Action("Index", "Home");
                window.location.href = "@Url.Action('Index', 'Home')";
            }
        },
        error: function (e) {
            alert('Error');
        }
    });
});

// Show modal
function showModal(header, body) {
    $('#insuranceModal .modal-dialog .modal-content .modal-header h4').text(header);
    $('#insuranceModal .modal-dialog .modal-content .modal-body').empty();
    $('#insuranceModal .modal-dialog .modal-content .modal-body').append(body);
    $("#insuranceModal").modal("show");

    $(document).on("click", "#insuranceModal button.btnClose", function (e) {
        $("#insuranceModal").modal("hide");
    });
}

// Turn back Payment Information form from Order Summary form
$(document).on("click", "#orderSummary button.btnBack", function () {
    $('#purchaseInsurance button[data-bs-target="#tab-pane-4"]').removeClass('active');
    $('#purchaseInsurance button[data-bs-target="#tab-pane-3"]').addClass('active');
    $.ajax({
        url: '/PurchaseInsuranceProcesses/_PartialPaymentInfo?vehicleID=' + id + '&insuranceID=' + insuranceID + '',
        type: 'GET',
        cache: false,
        success: function (result) {
            $('#orderForm').html(result);
            $('#contentOrder div.insurancePrice div.content span').text(formatCurrency(insurancePrice));
            loadPaymentInfoScript();

            loadPaymentInfoAgain();
        },
        error: function (e) {
            console.log(e);
        }
    });
});