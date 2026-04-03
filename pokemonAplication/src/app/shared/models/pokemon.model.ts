export interface Pokemon {
    id: number;
    name: string;
    species: string;
    level: number;
    clinicalStatus: string;
    trainerId: number;
    imageUrl?: string;
    types?: string[];
}

export interface PokemonApiResponse {
    id: number;
    name: string;
    sprites: {
        front_default: string;
        other: {
            'official-artwork': {
                front_default: string;
            };
        };
    };
    types: {
        type: {
            name: string;
        };
    }[];
}

export interface Trainer {
    id: number;
    name: string;
    email: string;
    phone: string;
}

export interface Treatment {
    id: number;
    pokemonId: number;
    type: string;
    dose: string;
    frequency: string;
    startDate: string;
    endDate: string;
    status: string;
}

export interface Appointment {
    id: number;
    pokemonId: number;
    date: string;
    reason: string;
    status: string;
}