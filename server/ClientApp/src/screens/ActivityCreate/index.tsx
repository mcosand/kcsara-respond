import React from 'react';
import { Autocomplete,  Button, Card, CardContent, CardActions, Checkbox, FormControl, FormControlLabel, FormHelperText, 
  FormLabel, FormGroup, IconButton, TextField, Grid } from '@material-ui/core';
import MapIcon from '@material-ui/icons/Map';
import { observer } from 'mobx-react';

import { MobileDateTimePicker as DateTimePicker } from '@material-ui/lab';
import { MainStore } from '../../store/main-store';
import { ActivityCreateUIStore } from './uiStore';

export const ActivityCreate: React.FC<{ store: MainStore }> = ({ store }) => {
  const uiStore = React.useMemo(() => new ActivityCreateUIStore(store), [store]);

  return (
<Grid container spacing={2} justifyContent="center">
  <Grid item container direction="column" xs={12} sm={10} md={8} lg={6} xl={5}>
      <Card style={{ padding: '1em', flexGrow: 1 }}>
        <CardContent>
        <h3>Create an Activity</h3>
        {uiStore.locationText.error}
        <div>
          <TextField
            label="DEM Number"
            value={uiStore.demNumber.value}
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
            getOptionLabel={(option) => option.text}
            filterOptions={x => x}
            options={[{text:'Alpental Upper Lot', coords: [47.5, -121.52] as [number,number], wkid: 'abd'}]}
            isOptionEqualToValue={(option, value) => {
              return option?.text === value?.text
                && option?.coords?.[0] === value?.coords?.[0]
                && option?.coords?.[1] === value?.coords?.[1]
                && option?.wkid === value?.wkid;
            }}
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
                <li key={option.text} {...props}>{option.text} {JSON.stringify(option.coords)}</li>
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
  </Grid>
</Grid>
  );
}

export default observer(ActivityCreate);