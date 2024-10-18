"use strict";
var __createBinding = (this && this.__createBinding) || (Object.create ? (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    var desc = Object.getOwnPropertyDescriptor(m, k);
    if (!desc || ("get" in desc ? !m.__esModule : desc.writable || desc.configurable)) {
      desc = { enumerable: true, get: function() { return m[k]; } };
    }
    Object.defineProperty(o, k2, desc);
}) : (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    o[k2] = m[k];
}));
var __setModuleDefault = (this && this.__setModuleDefault) || (Object.create ? (function(o, v) {
    Object.defineProperty(o, "default", { enumerable: true, value: v });
}) : function(o, v) {
    o["default"] = v;
});
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (k !== "default" && Object.prototype.hasOwnProperty.call(mod, k)) __createBinding(result, mod, k);
    __setModuleDefault(result, mod);
    return result;
};
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.server = exports.startServer = void 0;
const data_source_1 = require("./data-source");
const express_1 = __importDefault(require("express"));
const body_parser_1 = __importDefault(require("body-parser"));
const ProductRoutes_1 = __importDefault(require("../shared/routes/ProductRoutes"));
const dotenv = __importStar(require("dotenv"));
dotenv.config();
const server = (0, express_1.default)();
exports.server = server;
const cors = require('cors');
server.use(cors({
    origin: 'http://localhost:5173' // Permite apenas esta origem
}));
server.use(body_parser_1.default.json());
server.get('/', (_req, res) => res.status(200).json({
    msg: "Welcome to menu-service"
}));
async function startServer() {
    try {
        await data_source_1.AppDataSource.initialize();
        console.log('Database connection established successfully.');
        server.listen(process.env.DB_PORT || 3000, () => {
            console.log(`DB Server is running on port ${process.env.DB_PORT || 3000}`);
        });
    }
    catch (error) {
        console.error('Error connecting to the database:', error);
    }
}
exports.startServer = startServer;
startServer();
server.use(ProductRoutes_1.default);
// Middleware para verificar a conexão com o banco
server.use((req, res, next) => {
    if (!data_source_1.AppDataSource.isInitialized) {
        return res.status(503).json({
            error: 'Service Unavailable: Unable to connect to the database. Contact support, please'
        });
    }
    next();
});
//# sourceMappingURL=Server.js.map