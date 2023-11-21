import { addToCart } from "./Cart/AddToCart.js";
import { renderCartQuantity } from "./Cart/RenderCartQuantity.js";
import { storage } from "./Cart/Storage.js";


const searchbar = $("#search-bar")
const resultList = $("#result-list")
const searchResult = $('#result')
const uid = $("#user-info").attr("data-uid")


function searchHandler() {

    const debounce = (fn, delay = 1000) => {
        let timerId = null;
        return (...args) => {
            clearTimeout(timerId);
            timerId = setTimeout(() => fn(...args), delay);
        };
    };

    const render = async (value) => {
        let data;

        if (value.length <= 0) {
            searchResult.css("display", "none") // hide 
            return;
        }


        try {
            // Show Loading
            $("#search-loading-bar").css("display", "block");

            await $.ajax({
                type: "GET",
                url: `/api/product/GetBySearch?searchValue=${value}`,
                success: (resData) => {
                    data = resData
                    searchResult.css("display", "block") // show 
                },
                cache: true
            })
        } catch(err) {
            $("#search-loading-bar").css("display", "none");
            resultList.css("text-align", "center")
            resultList.html("Đã xảy ra lỗi ⛔")
        }

        $("#search-loading-bar").css("display", "none");



        const htmls = data.map(item => {
            return `
                    <li class="mb-1 py-2 px-3">
                        <a class="d-flex align-items-center" href="/Product/Detail/${item.Id}">
                           <img width="50" height="50" class="object-fit-cover rounded-2 border" src="/Assets/ImageStorage/Products/${item.Image}" />
                           <div class="d-flex flex-column ms-3">
                                <h5 class="fs-6">${item.Name}</h5>
                                <p class="fs-6">${item.Price.toFixed(2)} đ</p>
                           </div>
                        </a>
                   </li>
                `
        })


        if (htmls.length > 0) {
            resultList.html(htmls.join(''))
        } else {
            resultList.css("text-align", "center")
            resultList.html("Không tìm thấy sản phẩm")
        }
    }

    const onSearch = debounce(render, 500)

    searchbar.on("input", (e) => {
        onSearch(e.target.value.toLocaleLowerCase())
        
    })
}

async function  start() {
    searchHandler()
    addToCart()

    if (uid) {
        renderCartQuantity();
    }
}

start()