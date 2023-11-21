import { storage } from "./Storage.js"

async function renderCartQuantity() {
    const quantityValues = document.querySelector('#cart-quantity')
    const uid = $("#user-info").attr("data-uid")

    let quantity = await storage.get(uid)
    quantityValues.innerText = quantity.length || 0
}

export { renderCartQuantity }