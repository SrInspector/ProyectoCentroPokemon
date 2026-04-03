import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import {
  IonHeader, IonToolbar, IonTitle, IonContent,
  IonCard, IonCardHeader, IonCardTitle, IonCardContent,
  IonBadge, IonButton, IonIcon, IonButtons,
  IonSpinner, IonItem, IonLabel, IonList,
  IonProgressBar
} from '@ionic/angular/standalone';
import { PokeapiService } from '../../core/services/pokeapi';

@Component({
  selector: 'app-estado-clinico',
  templateUrl: './estado-clinico.page.html',
  styleUrls: ['./estado-clinico.page.scss'],
  standalone: true,
  imports: [
    CommonModule,
    IonHeader, IonToolbar, IonTitle, IonContent,
    IonCard, IonCardHeader, IonCardTitle, IonCardContent,
    IonBadge, IonButton, IonIcon, IonButtons,
    IonSpinner, IonItem, IonLabel, IonList,
    IonProgressBar
  ]
})
export class EstadoClinicoPage implements OnInit {

  isLoading: boolean = true;
  pokemons: any[] = [];

  mockPokemons = [
    {
      name: 'pikachu',
      estadoClinico: 'Estable',
      nivel: 25,
      peso: 6.0,
      altura: 0.4,
      ultimaRevision: '2026-03-15',
      proximaCita: '2026-04-05',
      tratamientosActivos: 1,
      observaciones: 'Pokémon en buen estado general, continuar tratamiento.'
    },
    {
      name: 'charmander',
      estadoClinico: 'En tratamiento',
      nivel: 15,
      peso: 8.5,
      altura: 0.6,
      ultimaRevision: '2026-03-10',
      proximaCita: '2026-04-10',
      tratamientosActivos: 2,
      observaciones: 'Requiere terapia física por lesión en cola.'
    },
    {
      name: 'bulbasaur',
      estadoClinico: 'Estable',
      nivel: 18,
      peso: 6.9,
      altura: 0.7,
      ultimaRevision: '2026-03-20',
      proximaCita: '2026-04-15',
      tratamientosActivos: 0,
      observaciones: 'Completó tratamiento de vitaminas exitosamente.'
    }
  ];

  constructor(
    private pokeapiService: PokeapiService,
    private router: Router
  ) { }

  ngOnInit() {
    this.loadPokemons();
  }

  loadPokemons() {
    this.isLoading = true;
    const requests = this.mockPokemons.map(p =>
      this.pokeapiService.getPokemonByName(p.name).toPromise()
    );

    Promise.all(requests).then(results => {
      this.pokemons = results.map((data: any, index) => ({
        ...this.mockPokemons[index],
        image: data.sprites.other['official-artwork'].front_default,
        types: data.types.map((t: any) => t.type.name),
        stats: data.stats.map((s: any) => ({
          name: s.stat.name,
          value: s.base_stat
        }))
      }));
      this.isLoading = false;
    });
  }

  getEstadoColor(estado: string): string {
    switch (estado) {
      case 'Estable': return 'success';
      case 'En tratamiento': return 'warning';
      case 'Crítico': return 'danger';
      default: return 'medium';
    }
  }

  goBack() {
    this.router.navigate(['/pokemon-list']);
  }
}