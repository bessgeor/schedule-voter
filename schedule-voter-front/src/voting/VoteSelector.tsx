import './voteSelector.css'
import React from 'react'
import * as moment from 'moment'
import { Vote, voteToString, Voting } from './types'

export type VoteSelection = {
	date: string;
	actualVotes: Voting[]
}

type VoterProps = {
	vote: Voting
	onVote: (vote: Voting) => void
}

const votes = [
	Vote.CantAttend,
	Vote.MayAttend,
	Vote.WantAttend
]

function Voter({ vote, onVote }: VoterProps) {
	const className = React.useMemo(() => `voter voter__vote-${vote.vote}`, [vote.vote])
	return (
		<div className={className}>
			<label>{moment.utc(vote.tourney).local().format("HH:mm")}</label>
			<select value={vote.vote} onChange={v => onVote({ tourney: vote.tourney, vote: votes[Number(v.target.value)] })}>
				{votes.map(v => <option key={v} value={v}>{voteToString(v)}</option>)}
			</select>
		</div>
	)
}

export type VoteSelectorProps = {
	selection: VoteSelection
	onVote: (vote: Voting) => void
	onClose: () => void
}	

export function VoteSelector({ selection, onVote, onClose }: VoteSelectorProps) {
	return (
		<div className="vote-selector">
			<h1>{selection.date}</h1>
			<div className="vote-selector__date-list">
				{selection.actualVotes.map(v => <Voter key={v.tourney.format("HH:mm")} vote={v} onVote={onVote} />)}
			</div>
			<div className="submit-panel">
				<button type="button" className="submit-button" onClick={onClose}>Готово!</button>
			</div>
		</div>
	)
}