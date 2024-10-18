"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.toProductDto = toProductDto;
const productDto_1 = require("../dtos/productDto");
function toProductDto(product) {
    return product.map((product) => {
        return new productDto_1.ProductDto({
            id: product.id,
            name: product.name,
            description: product.description,
            image: product.image,
            price: product.price
        });
    });
}
;
//# sourceMappingURL=toProductDto.js.map