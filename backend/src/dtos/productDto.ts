
export class ProductDto {
  id?: string;
  name?: string;
  description?: string;
  image?: string;
  price?: number;

  constructor(prod){
    this.id = prod.id;
    this.name = prod.name;
    this.description = prod.description;
    this.image = prod.image;
    this.price = prod.price;
  }

  }



