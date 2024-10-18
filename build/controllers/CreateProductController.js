"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.CreateProductsController = void 0;
const data_source_1 = require("../database/data-source");
const Product_1 = require("../entity/Product");
class CreateProductsController {
    async createProduct(req, res) {
        const productRepository = data_source_1.AppDataSource.getRepository(Product_1.Product);
        let newProduct;
        try {
            newProduct = productRepository.create(req.body);
            await productRepository.save(newProduct);
        }
        catch (exception) {
            return res.status(500).json("something went wrong: " + exception);
        }
        return res.json(newProduct);
    }
}
exports.CreateProductsController = CreateProductsController;
//# sourceMappingURL=CreateProductController.js.map