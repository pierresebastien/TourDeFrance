/// <reference path="../references.ts"/>

class Stage {
    id: string;
    name: string;
    ownerId: string;
    duration: number;
}

class StageDrink {
    id: string;
    drinkId: string;
    drinkName: string;
    alcoholByVolume: number;
    volume: number;
    isComposedDrink: boolean;
    stageId: string;
    overridedVolume: number;
    numberToDrink: number;
    order: number;
    type: StageType;
    gameVolume: number;
}