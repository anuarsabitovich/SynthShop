import { Product } from "./product"

export interface Basket {
    basketId: string
    items: BasketItem[]
  }
  
  export interface BasketItem {
    basketItemId: string
    quantity: number
    productId: string
    product: Product
    basketId: string
  } 