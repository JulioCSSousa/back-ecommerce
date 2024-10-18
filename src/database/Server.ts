import { AppDataSource } from "./data-source";
import express from 'express';
import bodyParser from 'body-parser';
import productRoutes from '../shared/routes/ProductRoutes';
import * as dotenv from "dotenv";
import cors from 'cors';


dotenv.config();
const server = express();

server.use(cors());
server.use(bodyParser.json());

server.get('/', (_req, res) => res.status(200).json({
  msg: "Welcome to menu-service"
}));

export async function startServer() {
  try {
    await AppDataSource.initialize();
    console.log('Database connection established successfully.');

    server.listen(process.env.DB_PORT || 3000, () => {
      console.log(`DB Server is running on port ${process.env.DB_PORT || 3000}`);
    });
  } catch (error) {
    console.error('Error connecting to the database:', error);
  }
}

startServer()
server.use(productRoutes);


// Middleware para verificar a conexão com o banco
server.use((req, res, next) => {
  if (!AppDataSource.isInitialized) {
    return res.status(503).json({
      error: 'Service Unavailable: Unable to connect to the database. Contact support, please'
    });
  }
  next();
});

export { server }