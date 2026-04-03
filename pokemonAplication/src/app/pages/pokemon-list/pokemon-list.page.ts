import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import {
  IonHeader, IonToolbar, IonTitle, IonContent,
  IonCard, IonCardHeader, IonCardTitle, IonCardSubtitle,
  IonCardContent, IonBadge, IonButton, IonIcon,
  IonButtons, IonSpinner, IonImg
} from '@ionic/angular/standalone';
import { PokeapiService } from '../../core/services/pokeapi';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-pokemon-list',
  templateUrl: './pokemon-list.page.html',
  styleUrls: ['./pokemon-list.page.scss'],
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    IonHeader, IonToolbar, IonTitle, IonContent,
    IonCard, IonCardHeader, IonCardTitle, IonCardSubtitle,
    IonCardContent, IonBadge, IonButton, IonIcon,
    IonButtons, IonSpinner, IonImg
  ]
})
export class PokemonListPage implements OnInit {

  trainer: any;
  pokemons: any[] = [];
  isLoading: boolean = true;

  mockPokemons = [
    { id: 1, name: 'pikachu', level: 25, clinicalStatus: 'Estable' },
    { id: 2, name: 'charmander', level: 15, clinicalStatus: 'En tratamiento' },
    { id: 3, name: 'bulbasaur', level: 18, clinicalStatus: 'Estable' },
  ];

  constructor(
    private pokeapiService: PokeapiService,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit() {
    this.trainer = this.authService.getUser();
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
        types: data.types.map((t: any) => t.type.name)
      }));
      this.isLoading = false;
    });
  }

  goToDetail(pokemon: any) {
    this.router.navigate(['/pokemon-detail', pokemon.name]);
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}