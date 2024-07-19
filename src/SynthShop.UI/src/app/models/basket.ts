import { Product } from "./product"

export interface Basket {
    basketId: string
    CustomerId: string | null
    items: BasketItem[]
  }
  
  export interface BasketItem {
    basketItemId: string
    quantity: number
    productId: string
    product: Product
    basketId: string
  } 