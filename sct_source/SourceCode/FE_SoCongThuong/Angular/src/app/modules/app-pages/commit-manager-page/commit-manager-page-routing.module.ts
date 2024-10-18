import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CommitManagerPageComponent } from './commit-manager-page.component';
import { TableCommitManagerPageComponent } from './table-commit-manager-page/table-commit-manager-page.component';
const routes: Routes = [
  {
    path: '',
    component: CommitManagerPageComponent,
    children: [
      {
        path: 'list',
        component: TableCommitManagerPageComponent,
      },
      { path: '', redirectTo: 'list', pathMatch: 'full' },
      { path: '**', redirectTo: 'list', pathMatch: 'full' },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CommitManagerPageRoutingModule {}
