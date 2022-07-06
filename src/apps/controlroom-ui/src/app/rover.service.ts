import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { environment } from '../environments/environment';
import { DaprClient, HttpMethod } from "dapr-client"; 

import { RoverPosition } from './rover';

@Injectable({
  providedIn: 'root'
})
export class RoverService {

  private urlPrefix = environment.apiBaseUrl;  // URL to web api
  private daprHost = "127.0.0.1"; 
  private daprPort = "3500"; 
  private serviceAppId = "rover-service";

  httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
  };

  constructor(private http: HttpClient) { }

  // async getRovers(): Observable<string[]> {
  //   const client = new DaprClient(this.daprHost, this.daprPort); 
  //   const response = (await client.invoker.invoke(this.serviceAppId , "rovers" , HttpMethod.GET)) as string[];
  //   return of(response);
  // }

  async getRovers(): Promise<string[]> {
    const client = new DaprClient(this.daprHost, this.daprPort); 
    return client.invoker.invoke(this.serviceAppId , "rovers" , HttpMethod.GET) as Promise<string[]>;
  }

  getRoverPosition(): Observable<RoverPosition> {
    const url = `${this.urlPrefix}/position/last`;
    return this.http.get<RoverPosition>(url);
  }

  getRoverPositions(): Observable<RoverPosition[]> {
    const url = `${this.urlPrefix}/position`;
    return this.http.get<RoverPosition[]>(url);
  }

  takeoff(command: string[]): any {
    const url = `${this.urlPrefix}/takeoff`
    return this.http.post(url, command, this.httpOptions);
  }

  start(command: string[]): any {
    const url = `${this.urlPrefix}/move`
    return this.http.post(url, command, this.httpOptions);
  }

  explore(command: string[]): any {
    const url = `${this.urlPrefix}/explore`
    return this.http.post(url, this.httpOptions);
  }

  
}
