"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GetProductsByIdController = void 0;
const data_source_1 = require("../database/data-source");
const Product_1 = require("../entity/Product");
class GetProductsByIdController {
    async getProductById(req, res) {
        const productRepository = data_source_1.AppDataSource.getRepository(Product_1.Product);
        const product = await productRepository.findOne({
            where: { id: req.params.id },
        });
        if (!product) {
            return res.status(404).json({ message: 'Product not found' });
        }
        return res.json(product);
    }
}
exports.GetProductsByIdController = GetProductsByIdController;
//# sourceMappingURL=GetProductsByIdController.js.map