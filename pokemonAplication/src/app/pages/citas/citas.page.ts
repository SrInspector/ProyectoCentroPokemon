import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import {
  IonHeader, IonToolbar, IonTitle, IonContent,
  IonCard, IonCardHeader, IonCardTitle, IonCardContent,
  IonBadge, IonButton, IonIcon, IonButtons,
  IonSpinner, IonItem, IonLabel, IonList
} from '@ionic/angular/standalone';
import { PokeapiService } from '../../core/services/pokeapi';

@Component({
  selector: 'app-citas',
  templateUrl: './citas.page.html',
  styleUrls: ['./citas.page.scss'],
  standalone: true,
  imports: [
    CommonModule,
    IonHeader, IonToolbar, IonTitle, IonContent,
    IonCard, IonCardHeader, IonCardTitle, IonCardContent,
    IonBadge, IonButton, IonIcon, IonButtons,
    IonSpinner, IonItem, IonLabel, IonList
  ]
})
export class CitasPage implements OnInit {

  isLoading: boolean = true;
  citas: any[] = [];

  mockCitas = [
    {
      id: 1,
      pokemonName: 'pikachu',
      fecha: '2026-04-05',
      hora: '09:00 AM',
      motivo: 'Revisión general',
      area: 'Consulta externa',
      estado: 'Confirmada'
    },
    {
      id: 2,
      pokemonName: 'charmander',
      fecha: '2026-04-10',
      hora: '11:00 AM',
      motivo: 'Control de tratamiento',
      area: 'Terapia física',
      estado: 'Pendiente'
    },
    {
      id: 3,
      pokemonName: 'bulbasaur',
      fecha: '2026-03-20',
      hora: '02:00 PM',
      motivo: 'Chequeo rutinario',
      area: 'Consulta externa',
      estado: 'Atendida'
    }
  ];

  constructor(
    private pokeapiService: PokeapiService,
    private router: Router
  ) { }

  ngOnInit() {
    this.loadCitas();
  }

  loadCitas() {
    this.isLoading = true;
    const requests = this.mockCitas.map(c =>
      this.pokeapiService.getPokemonByName(c.pokemonName).toPromise()
    );

    Promise.all(requests).then(results => {
      this.citas = results.map((data: any, index) => ({
        ...this.mockCitas[index],
        pokemonImage: data.sprites.other['official-artwork'].front_default
      }));
      this.isLoading = false;
    });
  }

  getEstadoColor(estado: string): string {
    switch (estado) {
      case 'Confirmada': return 'success';
      case 'Pendiente': return 'warning';
      case 'Cancelada': return 'danger';
      case 'Atendida': return 'primary';
      case 'Reprogramada': return 'tertiary';
      default: return 'medium';
    }
  }

  goBack() {
    this.router.navigate(['/pokemon-list']);
  }
}