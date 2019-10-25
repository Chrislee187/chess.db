import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IDashletData } from '../../dashlet/IDashletData'
@Component({
  selector: 'app-player-dashboard',
  templateUrl: './player-dashboard.component.html',
//  styleUrls: ['./dashboard.component.css']
})
export class PlayerDashboardComponent {
  private http: HttpClient;
  private baseUrl: string;

  public total: number;
  public uncheckedPlayers: IUncheckedPlayer[];

  constructor(http: HttpClient,
    @Inject('BASE_URL') baseUrl: string) {
    this.http = http;
    this.baseUrl = baseUrl;
  }

  ngOnInit() {
    this.getPlayerCount();
    this.getUncheckedPlayers();
  }

  getPlayerCount() {
    this.http.get<IDashletData>(this.baseUrl + 'api/dashboard/playersdata').subscribe(
      result => {
        this.total = result.totalCount;
      },
      error => {
        //        this.error = error;
        console.log(error.error);
      });
  }

  getUncheckedPlayers() {
    this.uncheckedPlayers = <IUncheckedPlayer[]>[
      { pgnName: 'Short, N' },
      { pgnName: 'Short, Nigel' },
      { pgnName: 'N Short' },
      { pgnName: 'N. Short' },
      { pgnName: 'Nigel Short' }
    ];
  }
}


interface IUncheckedPlayer {
  pgnName: string;
  firstName: string;
  middleName: string;
  lastName: string;
}
