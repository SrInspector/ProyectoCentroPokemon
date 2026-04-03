import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import {
  IonHeader, IonToolbar, IonTitle, IonContent,
  IonCard, IonCardHeader, IonCardTitle, IonCardContent,
  IonBadge, IonButton, IonIcon, IonButtons,
  IonSpinner, IonItem, IonLabel, IonList
} from '@ionic/angular/standalone';
import { AuthService } from '../../core/services/auth.service';
import { PokeapiService } from '../../core/services/pokeapi';

@Component({
  selector: 'app-tratamientos',
  templateUrl: './tratamientos.page.html',
  styleUrls: ['./tratamientos.page.scss'],
  standalone: true,
  imports: [
    CommonModule,
    IonHeader, IonToolbar, IonTitle, IonContent,
    IonCard, IonCardHeader, IonCardTitle, IonCardContent,
    IonBadge, IonButton, IonIcon, IonButtons,
    IonSpinner, IonItem, IonLabel, IonList
  ]
})
export class TratamientosPage implements OnInit {

  isLoading: boolean = true;
  tratamientos: any[] = [];

  // Datos simulados hasta tener backend
  mockTratamientos = [
    {
      id: 1,
      pokemonName: 'pikachu',
      tipo: 'Medicamento',
      dosis: '10mg',
      frecuencia: 'Cada 8 horas',
      fechaInicio: '2026-03-01',
      fechaFin: '2026-04-01',
      estado: 'Activo'
    },
    {
      id: 2,
      pokemonName: 'charmander',
      tipo: 'Terapia física',
      dosis: 'N/A',
      frecuencia: 'Diario',
      fechaInicio: '2026-03-10',
      fechaFin: '2026-03-30',
      estado: 'Activo'
    },
    {
      id: 3,
      pokemonName: 'bulbasaur',
      tipo: 'Vitaminas',
      dosis: '5mg',
      frecuencia: 'Una vez al día',
      fechaInicio: '2026-02-15',
      fechaFin: '2026-03-15',
      estado: 'Finalizado'
    }
  ];

  constructor(
    private authService: AuthService,
    private pokeapiService: PokeapiService,
    private router: Router
  ) { }

  ngOnInit() {
    this.loadTratamientos();
  }

  loadTratamientos() {
    this.isLoading = true;
    const requests = this.mockTratamientos.map(t =>
      this.pokeapiService.getPokemonByName(t.pokemonName).toPromise()
    );

    Promise.all(requests).then(results => {
      this.tratamientos = results.map((data: any, index) => ({
        ...this.mockTratamientos[index],
        pokemonImage: data.sprites.other['official-artwork'].front_default
      }));
      this.isLoading = false;
    });
  }

  getEstadoColor(estado: string): string {
    switch (estado) {
      case 'Activo': return 'warning';
      case 'Finalizado': return 'success';
      case 'Cancelado': return 'danger';
      default: return 'medium';
    }
  }

  goBack() {
    this.router.navigate(['/pokemon-list']);
  }
}