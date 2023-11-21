import { addToCart } from './Cart/AddToCart.js'
import { handleDecrease, handleIncrease } from './Cart/SelectQuantity.js'

function start() {
    handleDecrease()
    handleIncrease()

}

start()