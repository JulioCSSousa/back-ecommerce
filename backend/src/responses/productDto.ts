
export class ProductNoIDResponse {
  name?: string;
  description?: string;
  image?: string;
  price?: number;

  constructor(prod){
    this.name = prod.name;
    this.description = prod.description;
    this.image = prod.image;
    this.price = prod.price;
  }

}


