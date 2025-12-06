import { Component, EventEmitter, Output, Input, OnChanges } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, FormControl } from '@angular/forms';

@Component({
  selector: 'app-search-bar',
  standalone: false,
  templateUrl: './search-bar.component.html',
  styleUrl: './search-bar.component.scss'
})
export class SearchBarComponent {
  @Output() search = new EventEmitter<{ query: string; engines: string[] }>();
  @Input() engines: string[] = [];

  form: FormGroup;

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      query: [""],
      selectedEngines: this.fb.array([])
    });
  }

  ngOnChanges() {
    const selectedEnginesArray = this.form.get("selectedEngines") as FormArray;
    selectedEnginesArray.clear();
    this.engines.forEach(() => selectedEnginesArray.push(this.fb.control(true)));
  }

  get selectedEnginesControls() {
    return (this.form.get("selectedEngines") as FormArray).controls as FormControl[];
  }

  // Would use an Angular validator for a bigger project
  get isFormValid(): boolean {
    const query = this.form.get("query")?.value || "";
    const selectedEnginesArray = this.form.get("selectedEngines") as FormArray;
    const hasSelectedEngine = this.engines.some((_, i) => selectedEnginesArray.at(i).value);

    return query.trim().length >= 3 && hasSelectedEngine;
  }

  onSubmit() {
    if (!this.isFormValid) return;

    const query = this.form.get("query")?.value.trim();
    const selectedEnginesArray = this.form.get("selectedEngines") as FormArray;
    const selectedEngines = this.engines.filter((_, i) => selectedEnginesArray.at(i).value);

    this.search.emit({ query, engines: selectedEngines });
  }
}
