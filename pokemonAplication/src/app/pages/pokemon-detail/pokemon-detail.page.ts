import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import {
  IonHeader, IonToolbar, IonTitle, IonContent,
  IonCard, IonCardHeader, IonCardTitle, IonCardContent,
  IonBadge, IonButton, IonIcon, IonButtons,
  IonSpinner, IonProgressBar, IonItem, IonLabel
} from '@ionic/angular/standalone';
import { PokeapiService } from '../../core/services/pokeapi';

@Component({
  selector: 'app-pokemon-detail',
  templateUrl: './pokemon-detail.page.html',
  styleUrls: ['./pokemon-detail.page.scss'],
  standalone: true,
  imports: [
    CommonModule,
    IonHeader, IonToolbar, IonTitle, IonContent,
    IonCard, IonCardHeader, IonCardTitle, IonCardContent,
    IonBadge, IonButton, IonIcon, IonButtons,
    IonSpinner, IonProgressBar, IonItem, IonLabel
  ]
})
export class PokemonDetailPage implements OnInit {

  pokemon: any = null;
  isLoading: boolean = true;
  pokemonName: string = '';

  // Datos clínicos simulados
  clinicalData = {
    clinicalStatus: 'Estable',
    level: 25,
    trainer: 'Ash Ketchum',
    lastVisit: '2026-03-15',
    activeTreatments: 1
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private pokeapiService: PokeapiService
  ) { }

  ngOnInit() {
    this.pokemonName = this.route.snapshot.paramMap.get('id') || '';
    this.loadPokemon();
  }

  loadPokemon() {
    this.pokeapiService.getPokemonByName(this.pokemonName).subscribe({
      next: (data: any) => {
        this.pokemon = {
          id: data.id,
          name: data.name,
          image: data.sprites.other['official-artwork'].front_default,
          types: data.types.map((t: any) => t.type.name),
          stats: data.stats.map((s: any) => ({
            name: s.stat.name,
            value: s.base_stat
          })),
          height: data.height / 10,
          weight: data.weight / 10,
        };
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  goBack() {
    this.router.navigate(['/pokemon-list']);
  }
}