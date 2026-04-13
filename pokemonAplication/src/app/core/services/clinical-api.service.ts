import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ClinicalApiService {
  private readonly baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getPokemons(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Pokemon`);
  }

  getPokemon(id: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/Pokemon/${id}`);
  }

  getTratamientos(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Tratamientos`);
  }

  getCitas(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Citas`);
  }

  getFacturas(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Facturacion/facturas`);
  }

  downloadComprobante(id: number): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/Facturacion/facturas/${id}/comprobante`, { responseType: 'blob' });
  }

  getHistorial(pokemonId: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/Historial/pokemon/${pokemonId}`);
  }

  getEntrenadores(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Entrenadores`);
  }

  createCita(cita: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Citas`, cita);
  }

  createTratamiento(tratamiento: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Tratamientos`, tratamiento);
  }
}
