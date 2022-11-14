import React from "react";
import * as signalR from "@microsoft/signalr";

export const connection = new signalR.HubConnectionBuilder().withUrl("/liveHub").withAutomaticReconnect().build();



