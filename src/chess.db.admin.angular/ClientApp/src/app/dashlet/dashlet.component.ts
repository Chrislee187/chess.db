import { Component, Inject, Input } from "@angular/core";
import { HttpClient } from "@angular/common/http";

@Component({
  selector: 'dashlet',
  inputs: ['name', 'data'],
  templateUrl: './dashlet.component.html',
  styleUrls: ['../dashboard/dashboard.component.css']
})

export class DashletComponent {
  private baseUrl: string;
  private http: HttpClient;
  @Input() name: string;
  @Input() public endpoint: string;

  public totalCount: number;
  public error: string;
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
    this.http = http;
  }

  ngOnInit() {
    this.http.get<DashletData>(this.baseUrl + this.endpoint).subscribe(
      result => {
        this.totalCount = result.totalCount;
      },
      error => {
        this.error = error;
        console.log(error.error);
      });
  }
}

interface DashletData {
  totalCount: number
}

