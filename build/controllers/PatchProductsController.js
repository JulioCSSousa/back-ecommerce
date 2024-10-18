"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.PatchProductsController = void 0;
const data_source_1 = require("../database/data-source");
const Product_1 = require("../entity/Product");
class PatchProductsController {
    async patchProduct(req, res) {
        try {
            const productRepository = data_source_1.AppDataSource.getRepository(Product_1.Product);
            const product = await productRepository.findOne({
                where: { id: req.params.id }
            });
            if (!product) {
                return res.status(404).json({ message: 'Product not found' });
            }
            Object.keys(req.body).forEach((key) => {
                product[key] = req.body[key];
            });
            const result = await productRepository.save(product);
            return res.json(result);
        }
        catch (error) {
            console.error(error);
            return res.status(500).json({ error: 'error to update product' });
        }
    }
}
exports.PatchProductsController = PatchProductsController;
//# sourceMappingURL=PatchProductsController.js.map