import { Component, OnInit } from "@angular/core";
import { DashboardService } from "../services/DashboardService";

@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
  styleUrls: ["./home.component.css"]
})
export class HomeComponent implements OnInit {

  gamesCount: number;
  playersCount: number;
  sitesCount: number;
  eventsCount: number;

  constructor(private dashboardService: DashboardService) { }


  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.dashboardService.dashboardSummary()
      .subscribe({
        next: data => {
          this.gamesCount = data.gamesCount;
          this.playersCount = data.playersCount;
          this.sitesCount = data.sitesCount;
          this.eventsCount = data.eventsCount;

        },
        error: error => {
          console.log("ErRoR: ", error);
        }
      });
  }
}
