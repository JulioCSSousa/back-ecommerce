"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ProductDto = void 0;
class ProductDto {
    id;
    name;
    description;
    image;
    price;
    constructor(prod) {
        this.id = prod.id;
        this.name = prod.name;
        this.description = prod.description;
        this.image = prod.image;
        this.price = prod.price;
    }
}
exports.ProductDto = ProductDto;
//# sourceMappingURL=productDto.js.map