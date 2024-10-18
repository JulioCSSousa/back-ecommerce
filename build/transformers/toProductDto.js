"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.toProductDto = void 0;
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
exports.toProductDto = toProductDto;
;
//# sourceMappingURL=toProductDto.js.map