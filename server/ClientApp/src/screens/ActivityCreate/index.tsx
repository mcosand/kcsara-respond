import React from 'react';
import { Autocomplete,  Button, Card, CardContent, CardActions, Checkbox, FormControl, FormControlLabel, FormHelperText, 
  FormLabel, FormGroup, IconButton, TextField, Grid } from '@material-ui/core';
import MapIcon from '@material-ui/icons/Map';
import { observer } from 'mobx-react';

import { MobileDateTimePicker as DateTimePicker } from '@material-ui/lab';
import { MainStore } from '../../store/main-store';
import { ActivityCreateUIStore } from './uiStore';
import { Page } from '../../components/Page';

export const ActivityCreate: React.FC<{ store: MainStore }> = ({ store }) => {
  const uiStore = React.useMemo(() => new ActivityCreateUIStore(store), [store]);

  return (
    <Page store={store}>
      <Card style={{ flexGrow: 1 }}>
        <CardContent style={{paddingTop:0}}>
        <h3 style={{marginTop: '.5rem'}}>Create an Activity</h3>
        {uiStore.locationText.error}
        <div>
          <TextField
            label="DEM Number"
            value={uiStore.demNumber.value ?? ''}
            error={!!uiStore.demNumber.error}
            helperText={uiStore.demNumber.error}
            onChange={evt => uiStore.setNumber(evt.currentTarget.value)}
            onBlur={() => uiStore.validations.number()}
            margin="normal"
          />
        </div>
        <div>
          <TextField
            label="Title"
            value={uiStore.title.value}
            error={!!uiStore.title.error}
            helperText={uiStore.title.error}
            onChange={evt => uiStore.setTitle(evt.currentTarget.value)}
            onBlur={() => uiStore.validations.title()}
            required
            fullWidth
            margin="normal"
          />
        </div>
        <div>
          <DateTimePicker
            renderInput={(props) => <TextField {...props} margin="normal" />}
            label="Activity Start"
            value={uiStore.startTime}
            onChange={date => uiStore.setDate(date)}
            showTodayButton
            ampm={false}
          />
        </div>
        <div style={{display: 'flex', alignItems: 'center'}}>
          <Autocomplete
            getOptionLabel={(option) => option.properties.title}
            filterOptions={x => x}
            options={uiStore.locationSearchResults.obj ?? []}
            isOptionEqualToValue={uiStore.areLocationsEqual}
            autoComplete
            includeInputInList
            filterSelectedOptions
            sx={{flex:'1 1 auto'}}
            value={uiStore.selectedLocation}
            onChange={(_, newValue) => {
              uiStore.setSelectedLocation(newValue);
            }}
            onInputChange={(_, newInputValue) => {
              uiStore.setLocationText(newInputValue)
            }}
            renderInput={(params) => (
              <TextField {...params}
                label="Command Post / Staging"
                error={!!uiStore.locationText.error}
                helperText={uiStore.locationText.error}
                margin="normal"
                required />
            )}
            renderOption={(props, option) => {
              return (
                <li key={option.id} {...props}>{option.properties.title} {JSON.stringify(option.geometry.coordinates)}</li>
              )
            }}
          />        
          {/* <IconButton color="primary" aria-label="view map" component="div">
            <MapIcon />
          </IconButton> */}
        </div>
        <div>
          <FormControlLabel label="Create SARTopo Map" control={<Checkbox checked={uiStore.createMap} onChange={evt => uiStore.setCreateMap(evt.currentTarget.checked)} />}/>
        </div>
        <div>
          <FormControl required component="fieldset" variant="standard" error={!!uiStore.selectedUnits.error}>
            <FormLabel component="legend">Requested Units</FormLabel>
            <FormGroup>
              {uiStore.availableUnits.map(u => (
                <FormControlLabel key={u.id} label={u.name} control={
                  <Checkbox name={u.id} checked={!!uiStore.selectedUnits.value[u.id]} onChange={evt => uiStore.toggleUnit(evt.currentTarget.name, evt.currentTarget.checked)} />
                }/>
              ))}
            </FormGroup>
            {uiStore.selectedUnits.error ? <FormHelperText>{uiStore.selectedUnits.error}</FormHelperText> : undefined}
          </FormControl>
        </div>
        <div>{uiStore.saveError}</div>
        </CardContent>
        <CardActions>
          <Button size="small" disabled={uiStore.saving} onClick={() => uiStore.submit()}>Create</Button>
        </CardActions>
      </Card>
    </Page>
  );
}

export default observer(ActivityCreate);