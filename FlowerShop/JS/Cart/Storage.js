import { renderCartQuantity } from "./RenderCartQuantity.js";


export const storage = {
    async get(userId) {
        let data
        try {
            await $.ajax({
                type: "GET",
                url: `/api/Cart/GetCartItems?userId=${userId}`,
                success: (resData) => {
                    data = resData;
                },
                cache: true
            })
        } catch (err) {
            
        }

        return data;
    },
    async set(userId, productId, quantity) {
        let data
        try {
            await $.ajax({
                type: "POST",
                url: `/api/Cart/Post?userId=${userId}&productId=${productId}&quantity=${quantity}`,
                success: (resData) => {
                    // Render quantity
                    renderCartQuantity();

                    data = resData
                },
                cache: true
            })
        } catch (e) {

        }

        return data;

    },
   
    async updateQuantity(userId, productId, quantity) {
        let data
        try {
            await $.ajax({
                type: "PUT",
                url: `/api/Cart/Put?userId=${userId}&productId=${productId}&quantity=${quantity}`,
                success: (resData) => {
                    // Render quantity
                    renderCartQuantity();

                    data = resData
                },
                cache: true
            })
        } catch (e) {

        }

        return data;
    },
    async updateIncreaseQuantity(userId, productId) {
        let data;
        try {
            await $.ajax({
                type: "PUT",
                url: `/api/Cart/PutIncreaseQuantity?userId=${userId}&productId=${productId}`,
                success: (resData) => {
                    // Render quantity
                    renderCartQuantity();

                    data = resData
                },
                cache: true
            })
        } catch (e) {

        }

        return data;
    },
    async updateDecreaseQuantity(userId, productId) {
        let data;
        try {
            await $.ajax({
                type: "PUT",
                url: `/api/Cart/PutDecreaseQuantity?userId=${userId}&productId=${productId}`,
                success: (resData) => {
                    // Render quantity
                    renderCartQuantity();

                    data = resData
                },
                cache: true
            })
        } catch (e) {

        }

        return data;
    },
    async delete(userId, productId) {
        let data;
        try {
            await $.ajax({
                type: "DELETE",
                url: `/api/Cart/Delete?userId=${userId}&productId=${productId}`,
                success: (resData) => {
                    // Render quantity
                    renderCartQuantity();

                    data = resData
                },
                cache: true
            })
        } catch (e) {

        }

        return data;
    },
    clear() {

    }
}
