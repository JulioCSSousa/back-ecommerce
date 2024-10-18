"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.DeleteProductsController = void 0;
const data_source_1 = require("../database/data-source");
const Product_1 = require("../entity/Product");
class DeleteProductsController {
    async deleteProduct(req, res) {
        const productRepository = data_source_1.AppDataSource.getRepository(Product_1.Product);
        const result = await productRepository.delete(req.params.id);
        if (!result) {
            return res.status(404).json({ message: 'Product not found' });
        }
        return res.status(204).json({ message: "Successful deleted" });
    }
}
exports.DeleteProductsController = DeleteProductsController;
//# sourceMappingURL=DeleteProductsController.js.map