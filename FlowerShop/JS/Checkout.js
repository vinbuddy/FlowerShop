let shippingCost = 0;
let totalPayment = 0;

function selectCityDistrictWard() {
    var citis = document.getElementById("city");
    var districts = document.getElementById("district");
    var wards = document.getElementById("ward");
    var Parameter = {
        url: "https://raw.githubusercontent.com/kenzouno1/DiaGioiHanhChinhVN/master/data.json",
        method: "GET",
        responseType: "application/json",
    };
    var promise = axios(Parameter);
    promise.then(function (result) {
        renderCity(result.data);
    });

    function renderCity(data) {
        for (const x of data) {
            citis.options[citis.options.length] = new Option(x.Name, x.Id);
        }
        citis.onchange = function () {
            district.length = 1;
            ward.length = 1;
            if (this.value != "") {
                const result = data.filter(n => n.Id === this.value);

                for (const k of result[0].Districts) {
                    district.options[district.options.length] = new Option(k.Name, k.Id);
                }
            }

            var options = this.getElementsByTagName("option");
            var optionValue = options[this.selectedIndex].innerHTML;

            const citySelectRef = document.querySelector('select[name=City]')
            var cityOptions = citySelectRef.getElementsByTagName("option");
            var cityOption = cityOptions[citySelectRef.selectedIndex];
            cityOption.value = optionValue

            calculateShippingCost(optionValue)
            calculateTotalPayment()
        };
        district.onchange = function () {
            ward.length = 1;
            const dataCity = data.filter((n) => n.Id === citis.value);
            if (this.value != "") {
                const dataWards = dataCity[0].Districts.filter(n => n.Id === this.value)[0].Wards;

                for (const w of dataWards) {
                    wards.options[wards.options.length] = new Option(w.Name, w.Id);
                }

                var options = this.getElementsByTagName("option");
                var optionValue = options[this.selectedIndex].innerHTML;

                const districtSelectRef = document.querySelector('select[name=District]')
                var districtOptions = districtSelectRef.getElementsByTagName("option");
                var districtOption = districtOptions[districtSelectRef.selectedIndex];
                districtOption.value = optionValue


            }
        };

        wards.onchange = function () {
            if (this.value != "") {

                var options = this.getElementsByTagName("option");
                var optionValue = options[this.selectedIndex].innerHTML;

                const wardSelectRef = document.querySelector('select[name=Ward]')
                var wardOptions = wardSelectRef.getElementsByTagName("option");
                var wardOption = wardOptions[wardSelectRef.selectedIndex];
                wardOption.value = optionValue

            }
        };
    }
}

function renderDate() {
    const dateList = document.getElementById("date-list")
    const currentDateElement = document.getElementById("current-date")

    const currentDate = new Date();
    let date = new Date();
    
    let dateHtml = ""
    for (var i = 1; i < 7; i++) {
        date.setDate(date.getDate() + 1)
        dateHtml += `<label for="" class="date-label cursor-pointer bg-white p-2 border rounded-5 me-3">${new Date(date.getTime() + i - (date.getTimezoneOffset() * 60000)).toISOString().slice(0, 10)}</label>
        `
        
    }
    dateList.innerHTML = dateHtml;
    currentDateElement.innerHTML = `${new Date(currentDate.getTime() + 1 - (currentDate.getTimezoneOffset() * 60000)).toISOString().slice(0, 10)}`
}

function selectDate() {
    const dateLabels = document.querySelectorAll('.date-label')
    const dateInput = document.getElementById("RecieveDate")

    // Default select 
    if (dateInput.value.length == 0) {
        dateInput.value = document.getElementById("current-date").innerHTML
    }

    dateLabels.forEach((label, index) => {
        label.onclick = function () {
            const activeLabel = document.querySelector('.date-label.active')
            activeLabel.classList.remove('active')

            this.classList.add("active")

            // Pass date value
            dateInput.value = this.innerHTML.trim()
        }
    })
}

function selectTime() {
    const timeLabels = document.querySelectorAll('.time-label')
    const timeInput = document.getElementById("RecieveTime")

    // Default select 
    if (timeInput.value.length == 0) {
        timeInput.value = timeLabels[0].innerHTML.trim()
    }

    timeLabels.forEach((label, index) => {
        label.onclick = function () {
            const activeLabel = document.querySelector('.time-label.active')
            activeLabel.classList.remove('active')

            this.classList.add("active")

            // Pass date value
            timeInput.value = this.innerHTML.trim()
        }
    })
}

function formatVNCurrency(n) {
    return n.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
}

function calculateShippingCost(cityName = "") {
    const shippingCostElement = document.getElementById("shipping-cost")
    const shippingCostValue = document.querySelector("input[name=ShippingCost]")


    if (cityName.toLowerCase() != "thành phố hồ chí minh") {
        shippingCost = 30
    } else {
        shippingCost = 0
    }

    shippingCostElement.innerHTML = formatVNCurrency(shippingCost) + " đ"
    shippingCostValue.value = shippingCost
}

function calculateTotalPayment() {
    const totalPaymentValue = document.querySelector("input[name=TotalPayment]")
    const totalPriceItems = document.getElementById("total-price-items").value
    const totalPaymentElement = document.getElementById("total-payment")


    totalPayment = Number(totalPriceItems) + Number(shippingCost)

    totalPaymentValue.value = totalPayment
    totalPaymentElement.innerHTML = formatVNCurrency(totalPayment) + " đ"
}

function start() {
    selectCityDistrictWard()
    renderDate()
    selectDate()
    selectTime()
    calculateShippingCost()
    calculateTotalPayment()
}

start()