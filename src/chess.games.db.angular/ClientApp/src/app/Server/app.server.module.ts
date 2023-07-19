import { NgModule } from "@angular/core/core";
import { ServerModule } from "@angular/platform-server/platform-server";
import { ModuleMapLoaderModule } from "@nguniversal/module-map-ngfactory-loader/module-map-ngfactory-loader_public_index";
import { AppComponent } from "../app.component";
import { AppModule } from "../app.module";

@NgModule({
    imports: [AppModule, ServerModule, ModuleMapLoaderModule],
    bootstrap: [AppComponent]
})
export class AppServerModule { }
