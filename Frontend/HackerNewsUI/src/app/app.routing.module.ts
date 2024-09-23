import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NewsComponent } from './features/news/news.component';

const appRoutes: Routes = [
  {path: '', redirectTo: "news", pathMatch: 'full'},
  {path: 'news', component: NewsComponent},
];

@NgModule({
  imports: [RouterModule.forRoot(appRoutes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
