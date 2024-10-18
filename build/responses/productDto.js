"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ProductNoIDResponse = void 0;
class ProductNoIDResponse {
    name;
    description;
    image;
    price;
    constructor(prod) {
        this.name = prod.name;
        this.description = prod.description;
        this.image = prod.image;
        this.price = prod.price;
    }
}
exports.ProductNoIDResponse = ProductNoIDResponse;
//# sourceMappingURL=productDto.js.map