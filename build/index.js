"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const Server_1 = require("./database/Server");
const menuport = 8000;
const host = '0.0.0.0';
Server_1.server.listen(menuport, host, () => {
    console.log(`app server is running at ${menuport} ${host}`);
});
//# sourceMappingURL=index.js.map