import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {

  public gamesCount:number = 9999;
  public playersCount:number = 1234;
  public sitesCount:number = 9873;
  public eventsCount:number = 4321;
}
