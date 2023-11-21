import { storage } from './Cart/Storage.js';


const cartList = document.getElementById('cart-list')
const uid = $("#user-info").attr("data-uid")
const checkoutBtn = document.getElementById('checkout-btn')
const selectedItems = []


function formatVNCurrency(n) {
    return n.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
}

async function  renderCart() {
    const cart = await storage.get(uid)
    let products;

    try {
        await $.ajax({
            type: "GET",
            url: `/api/Product/Get`,
            success: (resData) => {
                products = resData
            },
            cache: true
        })
    } catch (err) {
        console.log(err)
    }


    const htmls = cart.map((cartItem) => {

        const productInCart = products.find(product => product.Id == cartItem.ProductId);


        return `
            <li data-id="${cartItem.ProductId}" class="cart-item cart-item-height d-flex mb-5">
                    <button ${productInCart.Quantity == 0 ? "disabled" : "" } style="width: 35px; height: 35px;" class="cart-item-select btn btn-sm-circle border">
                        <i class='pe-none bx bx-check'></i> 
                    </button>

                    <div class="flex-grow-1 d-flex justify-content-between ms-3">
                        <img style="opacity: ${productInCart.Quantity == 0 ? 0.64 : 1}" class="h-100 border rounded-10 object-fit-cover"
                             src="/Assets/ImageStorage/Products/${cartItem.ProductImage}"
                             alt="">
                        <div class="flex-grow-1 d-flex ms-4 w-100">
                            <div class="w-100 d-flex flex-column justify-content-between">
                                <div>
                                    <div class="d-flex align-items-center justify-content-between mb-3">
                                         <h5 style="opacity: ${productInCart.Quantity == 0 ? 0.64 : 1}">${cartItem.ProductName}</h5>
                                         <span class="${productInCart.Quantity == 0 ? "d-block" : "d-none"} px-3 py-1 rounded-pill shadow-sm bg-grey text-dark border">Bán hết</span>
                                    </div>
                                   
                                    <div style="opacity: ${productInCart.Quantity == 0 ? 0.64 : 1}" class="d-flex align-items-center justify-content-between">
                                        <div class="d-flex align-items-center ">
                                             <p class="${cartItem.ProductDiscountPrice ? "text-decoration-line-through" : ""} text-desc me-3">${formatVNCurrency(cartItem.ProductPrice)} đ</p>
                                            <p class="text-desc">${cartItem.ProductDiscountPrice ? formatVNCurrency(cartItem.ProductDiscountPrice) + " đ" : ""}</p>
                                        </div>
                                        <p class"mt-2">
                                            Tổng tiền: <span data-total="${cartItem.ProductDiscountPrice ? cartItem.TotalDiscount : cartItem.TotalPriceItem}" class="total-item-price">${cartItem.ProductDiscountPrice ? formatVNCurrency(cartItem.TotalDiscount) : formatVNCurrency(cartItem.TotalPriceItem)}</span> đ
                                        </p>
                                    </div>
                                </div>
                                <div class="d-flex justify-content-between align-items-center">
                                    <div style="opacity: ${productInCart.Quantity == 0 ? 0.64 : 1}" class="d-flex align-items-center border rounded-5 me-3">
                                        <button ${productInCart.Quantity == 0 ? "disabled" : "" } class="decrease-btn btn btn-sm">
                                            <i class=' bx bx-minus'></i>
                                        </button>
                                        <div class="quantity-select-value px-2">
                                            ${cartItem.Quantity}
                                        </div>
                                        <button ${productInCart.Quantity == 0 ? "disabled" : "" } class="increase-btn btn btn-sm">
                                            <i class='bx bx-plus'></i>
                                        </button>
                                    </div>
                                    <button  class="delete-cart-item fs-4 btn text-primary">
                                        <i class='bx bxs-trash'></i>
                                    </button>
                                </div>
                            </div>
                        </div>

                    </div>
                </li>
        `
    })

    if (htmls.length > 0) {
        cartList.innerHTML = htmls.join('')
    } else {
        cartList.innerHTML = `
            <img style="filter: grayscale(1)" width="200" class="d-block mx-auto" src="/Assets/Images/empty-cart.png" />
        `
    }
}

function UpdateQuantityIncrease() {

    const increaseBtn = $(".increase-btn")

    increaseBtn.click(async (e) => {
        const cartItem = e.target.closest(".cart-item")
        const productId = cartItem.dataset.id

        let response = await storage.updateIncreaseQuantity(uid, productId)
        response && await start()

    })

}

function UpdateQuantityDecrease() {

    const increaseBtn = $(".decrease-btn")

    increaseBtn.click(async (e) => {
        const cartItem = e.target.closest(".cart-item")
        const productId = cartItem.dataset.id

        let response = await storage.updateDecreaseQuantity(uid, productId)
        response && await start()

    })

}

function DeleteCartItem() {

    const deleteBtn = $(".delete-cart-item")

    deleteBtn.click(async (e) => {
        const cartItem = e.target.closest(".cart-item")
        const productId = cartItem.dataset.id

        let response = await storage.delete(uid, productId)
        response && await start()

    })

}

function SelectItem() {
    let totalPayment = 0;
    $(".cart-item-select").click((e) => {
        e.target.classList.toggle("active")
        
        const cartItem = e.target.closest(".cart-item")
        let totalItemPrice = cartItem.querySelector(".total-item-price").dataset.total
        console.log(totalItemPrice)

        if (e.target.classList.contains("active")) {
            totalPayment += Number(totalItemPrice)
        } else {
            totalPayment -= Number(totalItemPrice)
        }

        RenderTotalPayment(totalPayment)
        DisableCheckoutBtn()
        ToggleSelectProduct(cartItem.dataset.id)
    })

}

function ToggleSelectProduct(item) {
    if (selectedItems.includes(item)) {
        let index = selectedItems.indexOf(item);
        selectedItems.splice(index, 1)
    } else {
        selectedItems.push(item)
    }
}

function RenderTotalPayment(totalPayment = 0) {
    let value = totalPayment == 0 ? totalPayment : formatVNCurrency(totalPayment)
    $("#total-payment").html(value)
}

function DisableCheckoutBtn() {
    const checked = document.querySelectorAll('.cart-item-select.active')


    // Toggle disable checkout btn
    if (checked.length > 0) {
        checkoutBtn.disabled = false
    } else {
        checkoutBtn.disabled = true
    }

}

function SendSelectedItem(data) {
    $.ajax({
        url: '/Cart/SelectItem',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (res) {
            window.location.href = res.redirectToUrl;
        },
        failure: function () { 
            alert("not working...");
        }
    });
}

function Checkout() {
    checkoutBtn.onclick = function () {
        const data = {
            ItemSeleted: selectedItems
        }
        SendSelectedItem(data)
    }
}

async function start() {

    if (uid) {
        await renderCart()
        UpdateQuantityIncrease();
        UpdateQuantityDecrease();
        DeleteCartItem();
        SelectItem();
        RenderTotalPayment()
        DisableCheckoutBtn()
        Checkout()
    }

}

start()