import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { PokemonApiResponse } from '../../shared/models/pokemon.model';

@Injectable({
  providedIn: 'root'
})
export class PokeapiService {

  private apiUrl = 'https://pokeapi.co/api/v2';

  constructor(private http: HttpClient) { }

  getPokemonByName(name: string): Observable<PokemonApiResponse> {
    return this.http.get<PokemonApiResponse>(`${this.apiUrl}/pokemon/${name.toLowerCase()}`);
  }

  getPokemonById(id: number): Observable<PokemonApiResponse> {
    return this.http.get<PokemonApiResponse>(`${this.apiUrl}/pokemon/${id}`);
  }

  getPokemonImage(nameOrId: string | number): Observable<string> {
    return this.http.get<PokemonApiResponse>(`${this.apiUrl}/pokemon/${nameOrId}`).pipe(
      map(pokemon => pokemon.sprites.other['official-artwork'].front_default
        || pokemon.sprites.front_default)
    );
  }

  getPokemonTypes(nameOrId: string | number): Observable<string[]> {
    return this.http.get<PokemonApiResponse>(`${this.apiUrl}/pokemon/${nameOrId}`).pipe(
      map(pokemon => pokemon.types.map(t => t.type.name))
    );
  }
}